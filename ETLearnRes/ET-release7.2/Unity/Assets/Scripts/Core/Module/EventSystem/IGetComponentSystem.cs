using System;

namespace ET
{
    // 获取组件的接口，用于标识可以被获取的组件类型
    // GetComponentSystem 有巨大作用，比如每次保存 Unit 的数据不需要所有组件都保存，只需要保存 Unit 变化过的组件
    // 是否变化可以通过判断该组件是否已经被 GetComponentSystem，Get 了就记录该组件
    // 这样可以只保存 Unit 变化过的组件，再比如传送也可以做此类优化
    public interface IGetComponent
    {
    }

    // 获取组件的系统接口，继承自 ISystemType
    public interface IGetComponentSystem : ISystemType
    {
        // 运行获取组件系统的方法，传入实体对象和组件对象
        void Run(Entity entity, Entity component);
    }

    // 获取组件系统的抽象类，泛型参数 T 是 Entity 类型，且必须实现 IGetComponent 接口
    [ObjectSystem]
    public abstract class GetComponentSystem<T> : IGetComponentSystem where T : Entity, IGetComponent
    {
        // 实现 IGetComponentSystem 接口的 Run 方法
        void IGetComponentSystem.Run(Entity entity, Entity component)
        {
            // 转换实体类型为泛型类型 T，并调用子类重写的 GetComponentSystem 方法
            this.GetComponent((T)entity, component);
        }

        // 获取系统的类型，返回 IGetComponentSystem 的类型
        Type ISystemType.GetSystemType()
        {
            return typeof(IGetComponentSystem);
        }

        // 获取系统实例队列的索引，返回 InstanceQueueIndex.None，表示不使用队列
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // 获取系统处理的类型，返回泛型参数 T 的类型
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // 定义抽象方法，由子类实现具体的获取组件逻辑
        protected abstract void GetComponent(T self, Entity component);
    }
}
