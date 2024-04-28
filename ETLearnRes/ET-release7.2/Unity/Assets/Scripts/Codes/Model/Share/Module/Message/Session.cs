using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ET
{
    // RpcInfo结构体用于存储远程过程调用的信息
    public readonly struct RpcInfo
    {
        // Request字段存储远程调用的请求
        public readonly IRequest Request;

        // Tcs字段存储一个异步任务，用于等待远程调用的响应
        public readonly ETTask<IResponse> Tcs;

        // 构造函数，初始化RpcInfo结构体的实例
        public RpcInfo(IRequest request)
        {
            // 将传入的请求赋值给Request字段
            this.Request = request;
            // 创建一个异步任务，并赋值给Tcs字段
            this.Tcs = ETTask<IResponse>.Create(true);
        }
    }


    [FriendOf(typeof(Session))]
    public static class SessionSystem
    {
        [ObjectSystem]
        public class SessionAwakeSystem : AwakeSystem<Session, int>
        {
            protected override void Awake(Session self, int serviceId)
            {
                self.ServiceId = serviceId;
                long timeNow = TimeHelper.ClientNow();
                self.LastRecvTime = timeNow;
                self.LastSendTime = timeNow;

                self.requestCallbacks.Clear();

                Log.Info($"session create: zone: {self.DomainZone()} id: {self.Id} {timeNow} ");
            }
        }

        [ObjectSystem]
        public class SessionDestroySystem : DestroySystem<Session>
        {
            protected override void Destroy(Session self)
            {
                NetServices.Instance.RemoveChannel(self.ServiceId, self.Id, self.Error);

                foreach (RpcInfo responseCallback in self.requestCallbacks.Values.ToArray())
                {
                    responseCallback.Tcs.SetException(new RpcException(self.Error, $"session dispose: {self.Id} {self.RemoteAddress}"));
                }

                Log.Info($"session dispose: {self.RemoteAddress} id: {self.Id} ErrorCode: {self.Error}, please see ErrorCode.cs! {TimeHelper.ClientNow()}");

                self.requestCallbacks.Clear();
            }
        }

        public static void OnResponse(this Session self, IResponse response)
        {
            if (!self.requestCallbacks.TryGetValue(response.RpcId, out var action))
            {
                return;
            }

            self.requestCallbacks.Remove(response.RpcId);
            if (ErrorCore.IsRpcNeedThrowException(response.Error))
            {
                action.Tcs.SetException(new Exception($"Rpc error, request: {action.Request} response: {response}"));
                return;
            }
            action.Tcs.SetResult(response);
        }

        public static async ETTask<IResponse> Call(this Session self, IRequest request, ETCancellationToken cancellationToken)
        {
            int rpcId = ++Session.RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            self.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;

            self.Send(request);

            void CancelAction()
            {
                if (!self.requestCallbacks.TryGetValue(rpcId, out RpcInfo action))
                {
                    return;
                }

                self.requestCallbacks.Remove(rpcId);
                Type responseType = OpcodeTypeComponent.Instance.GetResponseType(action.Request.GetType());
                IResponse response = (IResponse)Activator.CreateInstance(responseType);
                response.Error = ErrorCore.ERR_Cancel;
                action.Tcs.SetResult(response);
            }

            IResponse ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await rpcInfo.Tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
            return ret;
        }

        public static async ETTask<IResponse> Call(this Session self, IRequest request)
        {
            int rpcId = ++Session.RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            self.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            self.Send(request);
            return await rpcInfo.Tcs;
        }

        public static void Send(this Session self, IMessage message)
        {
            self.Send(0, message);
        }

        public static void Send(this Session self, long actorId, IMessage message)
        {
            self.LastSendTime = TimeHelper.ClientNow();
            OpcodeHelper.LogMsg(self.DomainZone(), message);
            NetServices.Instance.SendMessage(self.ServiceId, self.Id, actorId, message);
        }
    }

    [ChildOf]
    // Session类表示一个会话，继承自Entity，并实现了IAwake<int>和IDestroy接口
    public sealed class Session : Entity, IAwake<int>, IDestroy
    {
        // ServiceId属性表示该会话的服务ID
        public int ServiceId { get; set; }

        // RpcId属性表示远程过程调用的ID
        public static int RpcId
        {
            get;
            set;
        }

        // requestCallbacks字段用于存储请求的回调信息
        public readonly Dictionary<int, RpcInfo> requestCallbacks = new Dictionary<int, RpcInfo>();

        // LastRecvTime属性表示上一次接收消息的时间
        public long LastRecvTime
        {
            get;
            set;
        }

        // LastSendTime属性表示上一次发送消息的时间
        public long LastSendTime
        {
            get;
            set;
        }

        // Error属性表示会话的错误码
        public int Error
        {
            get;
            set;
        }

        // RemoteAddress属性表示远程地址
        public IPEndPoint RemoteAddress
        {
            get;
            set;
        }
    }

}