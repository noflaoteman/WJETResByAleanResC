using System;

namespace ET
{
    public abstract class AMHandler<TMessage> : IMHandler where TMessage : class
    {
        protected abstract ETTask Run(Session session, TMessage message);

        public void Handle(Session session, object msg)
        {
            TMessage message = msg as TMessage;
            if (message == null)
            {
                Log.Error($"消息类型转换错误: {msg.GetType().Name} to {typeof(TMessage).Name}");
                return;
            }

            if (session.IsDisposed)
            {
                Log.Error($"session disconnect {msg}");
                return;
            }

            this.Run(session, message).Coroutine();
        }

        public Type GetMessageType()
        {
            return typeof(TMessage);
        }

        public Type GetResponseType()
        {
            return null;
        }
    }
}