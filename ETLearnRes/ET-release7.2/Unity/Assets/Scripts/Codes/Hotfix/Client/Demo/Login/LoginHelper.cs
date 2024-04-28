using System;
using System.Net;
using System.Net.Sockets;

namespace ET.Client

{
    public static class LoginHelper

    {
        // 定义公共静态异步方法Login，它接受一个场景、一个账户和一个密码作为参数。
        public static async ETTask Login(Scene clientScene, string account, string password)

        {
            try

            {
                // 从客户端场景中移除RouterAddressComponent组件。
                clientScene.RemoveComponent<RouterAddressComponent>();

                // 从客户端场景中获取RouterAddressComponent组件。
                RouterAddressComponent routerAddressComponent = clientScene.GetComponent<RouterAddressComponent>();

                // 如果RouterAddressComponent组件为空。
                if (routerAddressComponent == null)

                {
                    // 在客户端场景中添加一个RouterAddressComponent组件，并设置其主机和端口。
                    routerAddressComponent = clientScene.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);

                    // 等待初始化RouterAddressComponent组件。
                    await routerAddressComponent.Init();

                    // 在客户端场景中添加一个NetClientComponent组件，并设置其地址族。
                    clientScene.AddComponent<NetClientComponent, AddressFamily>(routerAddressComponent.RouterManagerIPAddress.AddressFamily);

                }
                // 从RouterAddressComponent组件中获取账户的Realm地址。
                IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

                // 定义一个R2C_Login变量。
                R2C_Login r2CLogin;

                // 创建一个路由会话。
                using (Session session = await RouterHelper.CreateRouterSession(clientScene, realmAddress))

                {
                    // 异步调用C2R_Login方法，并将结果转换为R2C_Login类型。(得到Response)
                    r2CLogin = (R2C_Login)await session.Call(new C2R_Login() { Account = account, Password = password });

                }

                // 创建一个网关会话。
                Session gateSession = await RouterHelper.CreateRouterSession(clientScene, NetworkHelper.ToIPEndPoint(r2CLogin.Address));

                // 在客户端场景中添加一个SessionComponent组件，并设置其会话为网关会话。
                clientScene.AddComponent<SessionComponent>().Session = gateSession;

                G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(
                    // 异步调用C2G_LoginGate方法，并将结果转换为G2C_LoginGate类型。
                    new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId });

                Log.Debug("登陆gate成功!");

                await EventSystem.Instance.PublishAsync(clientScene, new EventType.LoginFinish());

            }
            catch (Exception e)

            {
                Log.Error(e);

            }
        }
    }
}
