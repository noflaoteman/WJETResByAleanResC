using System;

namespace ET.Server

{
    [MessageHandler(SceneType.Gate)]

    public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGate, G2C_LoginGate> // 定义公共类C2G_LoginGateHandler，它继承自AMRpcHandler，用于处理从客户端到网关的登录请求。

    {
        protected override async ETTask Run(Session session, C2G_LoginGate request, G2C_LoginGate response) // 这是你需要重写的方法，它处理一个会话，一个请求和一个响应。

        {
            Scene scene = session.DomainScene(); // 从会话中获取场景。

            string account = scene.GetComponent<GateSessionKeyComponent>().Get(request.Key); // 从场景的GateSessionKeyComponent组件中获取账户。

            if (account == null) // 如果账户为空，即验证失败。

            {
                response.Error = ErrorCore.ERR_ConnectGateKeyError; // 设置响应的错误为连接网关键错误。

                response.Message = "Gate key验证失败!"; // 设置响应的消息为“Gate key验证失败!”。

                return; // 返回，不再执行后面的代码。

            }

            session.RemoveComponent<SessionAcceptTimeoutComponent>(); // 从会话中移除SessionAcceptTimeoutComponent组件。

            PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>(); // 从场景中获取PlayerComponent组件。

            Player player = playerComponent.AddChild<Player, string>(account); // 在PlayerComponent中添加一个新的Player子对象。

            playerComponent.Add(player); // 将新的Player对象添加到PlayerComponent中。

            session.AddComponent<SessionPlayerComponent>().PlayerId = player.Id; // 在会话中添加一个SessionPlayerComponent组件，并设置玩家ID。

            session.AddComponent<MailBoxComponent, MailboxType>(MailboxType.GateSession); // 在会话中添加一个MailBoxComponent组件，类型为GateSession。

            response.PlayerId = player.Id; // 设置响应的PlayerId为新创建的Player对象的Id。

            await ETTask.CompletedTask; // 异步等待任务完成。
        }
    }
}
