using System;

namespace ET
{
    public interface IInvoke
    {
        Type getType { get; }
    }

    public abstract class AInvokeHandler<T> : IInvoke where T : struct
    {
        public Type getType
        {
            get
            {
                return typeof(T);
            }
        }

        public abstract void Handle(T a);
    }

    public abstract class AInvokeHandler<A, T> : IInvoke where A : struct
    {
        public Type getType
        {
            get
            {
                return typeof(A);
            }
        }

        public abstract T Handle(A a);
    }
}