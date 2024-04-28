using System;

namespace ET
{
    public interface IDeserialize
    {
    }

    public interface IDeserializeSystem : ISystemType
    {
        void Run(Entity o);
    }

    /// <summary>
    /// 反序列化后执行的System
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ObjectSystem]
    public abstract class DeserializeSystem<T> : IDeserializeSystem where T : Entity, IDeserialize
    {
        void IDeserializeSystem.Run(Entity entity)
        {
            this.Deserialize((T)entity);
        }

        Type ISystemType.GetSystemType()
        {
            return typeof(IDeserializeSystem);
        }

        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        protected abstract void Deserialize(T self);
    }
}
