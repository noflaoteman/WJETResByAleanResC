using System;

namespace ET
{
    // 定义了一个不带参数的Awake接口
    public interface IAwake
    {
    }

    // 定义了一个带一个参数的Awake接口
    public interface IAwake<A>
    {
    }

    // 定义了一个带两个参数的Awake接口
    public interface IAwake<A, B>
    {
    }

    // 定义了一个带三个参数的Awake接口
    public interface IAwake<A, B, C>
    {
    }

    // 定义了一个带四个参数的Awake接口
    public interface IAwake<A, B, C, D>
    {
    }

    // 定义了不带参数的Awake系统接口
    public interface IAwakeSystem : ISystemType
    {
        // 运行Awake系统
        void Run(Entity o);
    }

    // 定义了带一个参数的Awake系统接口
    public interface IAwakeSystem<A> : ISystemType
    {
        // 运行Awake系统
        void Run(Entity o, A a);
    }

    // 定义了带两个参数的Awake系统接口
    public interface IAwakeSystem<A, B> : ISystemType
    {
        // 运行Awake系统
        void Run(Entity o, A a, B b);
    }

    // 定义了带三个参数的Awake系统接口
    public interface IAwakeSystem<A, B, C> : ISystemType
    {
        // 运行Awake系统
        void Run(Entity o, A a, B b, C c);
    }

    // 定义了带四个参数的Awake系统接口
    public interface IAwakeSystem<A, B, C, D> : ISystemType
    {
        // 运行Awake系统
        void Run(Entity o, A a, B b, C c, D d);
    }

    // Awake系统的基类，泛型类型为T，要求T必须是Entity类型且实现了IAwake接口
    [ObjectSystem]
    public abstract class AwakeSystem<T> : IAwakeSystem where T : Entity, IAwake
    {
        // 获取系统对应的类型
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // 获取系统类型
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem);
        }

        // 获取实例队列的索引
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // 运行Awake系统
        void IAwakeSystem.Run(Entity o)
        {
            this.Awake((T)o);
        }

        // 定义了一个抽象的Awake方法，需要在子类中实现
        protected abstract void Awake(T self);
    }

    // 带一个参数的Awake系统的基类，泛型类型为T和A，要求T必须是Entity类型且实现了IAwake<T>接口
    [ObjectSystem]
    public abstract class AwakeSystem<T, A> : IAwakeSystem<A> where T : Entity, IAwake<A>
    {
        // 获取系统对应的类型
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // 获取系统类型
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A>);
        }

        // 获取实例队列的索引
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // 运行Awake系统
        void IAwakeSystem<A>.Run(Entity entity, A a)
        {
            this.Awake((T)entity, a);
        }

        // 定义了一个抽象的Awake方法，需要在子类中实现
        protected abstract void Awake(T self, A a);
    }

    // 带两个参数的Awake系统的基类，泛型类型为T、A和B，要求T必须是Entity类型且实现了IAwake<T, B>接口
    [ObjectSystem]
    public abstract class AwakeSystem<T, A, B> : IAwakeSystem<A, B> where T : Entity, IAwake<A, B>
    {
        // 获取系统对应的类型
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // 获取系统类型
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A, B>);
        }

        // 获取实例队列的索引
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // 运行Awake系统
        void IAwakeSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.Awake((T)o, a, b);
        }

        // 定义了一个抽象的Awake方法，需要在子类中实现
        protected abstract void Awake(T self, A a, B b);
    }

    // 带三个参数的Awake系统的基类，泛型类型为T、T、B和C，要求T必须是Entity类型且实现了IAwake<T, B, C>接口
    [ObjectSystem]
    public abstract class AwakeSystem<T, A, B, C> : IAwakeSystem<A, B, C> where T : Entity, IAwake<A, B, C>
    {
        // 获取系统对应的类型
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // 获取系统类型
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A, B, C>);
        }

        // 获取实例队列的索引
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // 运行Awake系统
        void IAwakeSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.Awake((T)o, a, b, c);
        }

        // 定义了一个抽象的Awake方法，需要在子类中实现
        protected abstract void Awake(T self, A a, B b, C c);
    }

    // 带四个参数的Awake系统的基类，泛型类型为T、T、B、C和D，要求T必须是Entity类型且实现了IAwake<T, B, C, D>接口
    [ObjectSystem]
    public abstract class AwakeSystem<T, A, B, C, D> : IAwakeSystem<A, B, C, D> where T : Entity, IAwake<A, B, C, D>
    {
        // 获取系统对应的类型
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // 获取系统类型
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A, B, C, D>);
        }

        // 获取实例队列的索引
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // 运行Awake系统
        void IAwakeSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.Awake((T)o, a, b, c, d);
        }

        // 定义了一个抽象的Awake方法，需要在子类中实现
        protected abstract void Awake(T self, A a, B b, C c, D d);
    }
}
