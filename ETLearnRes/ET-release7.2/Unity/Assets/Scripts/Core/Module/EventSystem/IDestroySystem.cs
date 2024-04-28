using System;

namespace ET
{
    public interface IDestroy
    {
    }

    public interface IDestroySystem : ISystemType
    {
        void Run(Entity o);
    }

    [ObjectSystem]
    public abstract class DestroySystem<T> : IDestroySystem where T : Entity, IDestroy
    {
        void IDestroySystem.Run(Entity entity)
        {
            this.Destroy((T)entity);
        }

        Type ISystemType.GetSystemType()
        {
            return typeof(IDestroySystem);
        }

        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        protected abstract void Destroy(T self);
    }
}
