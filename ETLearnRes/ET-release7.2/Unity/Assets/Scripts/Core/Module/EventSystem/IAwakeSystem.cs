using System;

namespace ET
{
    // ������һ������������Awake�ӿ�
    public interface IAwake
    {
    }

    // ������һ����һ��������Awake�ӿ�
    public interface IAwake<A>
    {
    }

    // ������һ��������������Awake�ӿ�
    public interface IAwake<A, B>
    {
    }

    // ������һ��������������Awake�ӿ�
    public interface IAwake<A, B, C>
    {
    }

    // ������һ�����ĸ�������Awake�ӿ�
    public interface IAwake<A, B, C, D>
    {
    }

    // �����˲���������Awakeϵͳ�ӿ�
    public interface IAwakeSystem : ISystemType
    {
        // ����Awakeϵͳ
        void Run(Entity o);
    }

    // �����˴�һ��������Awakeϵͳ�ӿ�
    public interface IAwakeSystem<A> : ISystemType
    {
        // ����Awakeϵͳ
        void Run(Entity o, A a);
    }

    // �����˴�����������Awakeϵͳ�ӿ�
    public interface IAwakeSystem<A, B> : ISystemType
    {
        // ����Awakeϵͳ
        void Run(Entity o, A a, B b);
    }

    // �����˴�����������Awakeϵͳ�ӿ�
    public interface IAwakeSystem<A, B, C> : ISystemType
    {
        // ����Awakeϵͳ
        void Run(Entity o, A a, B b, C c);
    }

    // �����˴��ĸ�������Awakeϵͳ�ӿ�
    public interface IAwakeSystem<A, B, C, D> : ISystemType
    {
        // ����Awakeϵͳ
        void Run(Entity o, A a, B b, C c, D d);
    }

    // Awakeϵͳ�Ļ��࣬��������ΪT��Ҫ��T������Entity������ʵ����IAwake�ӿ�
    [ObjectSystem]
    public abstract class AwakeSystem<T> : IAwakeSystem where T : Entity, IAwake
    {
        // ��ȡϵͳ��Ӧ������
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // ��ȡϵͳ����
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem);
        }

        // ��ȡʵ�����е�����
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // ����Awakeϵͳ
        void IAwakeSystem.Run(Entity o)
        {
            this.Awake((T)o);
        }

        // ������һ�������Awake��������Ҫ��������ʵ��
        protected abstract void Awake(T self);
    }

    // ��һ��������Awakeϵͳ�Ļ��࣬��������ΪT��A��Ҫ��T������Entity������ʵ����IAwake<T>�ӿ�
    [ObjectSystem]
    public abstract class AwakeSystem<T, A> : IAwakeSystem<A> where T : Entity, IAwake<A>
    {
        // ��ȡϵͳ��Ӧ������
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // ��ȡϵͳ����
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A>);
        }

        // ��ȡʵ�����е�����
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // ����Awakeϵͳ
        void IAwakeSystem<A>.Run(Entity entity, A a)
        {
            this.Awake((T)entity, a);
        }

        // ������һ�������Awake��������Ҫ��������ʵ��
        protected abstract void Awake(T self, A a);
    }

    // ������������Awakeϵͳ�Ļ��࣬��������ΪT��A��B��Ҫ��T������Entity������ʵ����IAwake<T, B>�ӿ�
    [ObjectSystem]
    public abstract class AwakeSystem<T, A, B> : IAwakeSystem<A, B> where T : Entity, IAwake<A, B>
    {
        // ��ȡϵͳ��Ӧ������
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // ��ȡϵͳ����
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A, B>);
        }

        // ��ȡʵ�����е�����
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // ����Awakeϵͳ
        void IAwakeSystem<A, B>.Run(Entity o, A a, B b)
        {
            this.Awake((T)o, a, b);
        }

        // ������һ�������Awake��������Ҫ��������ʵ��
        protected abstract void Awake(T self, A a, B b);
    }

    // ������������Awakeϵͳ�Ļ��࣬��������ΪT��T��B��C��Ҫ��T������Entity������ʵ����IAwake<T, B, C>�ӿ�
    [ObjectSystem]
    public abstract class AwakeSystem<T, A, B, C> : IAwakeSystem<A, B, C> where T : Entity, IAwake<A, B, C>
    {
        // ��ȡϵͳ��Ӧ������
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // ��ȡϵͳ����
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A, B, C>);
        }

        // ��ȡʵ�����е�����
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // ����Awakeϵͳ
        void IAwakeSystem<A, B, C>.Run(Entity o, A a, B b, C c)
        {
            this.Awake((T)o, a, b, c);
        }

        // ������һ�������Awake��������Ҫ��������ʵ��
        protected abstract void Awake(T self, A a, B b, C c);
    }

    // ���ĸ�������Awakeϵͳ�Ļ��࣬��������ΪT��T��B��C��D��Ҫ��T������Entity������ʵ����IAwake<T, B, C, D>�ӿ�
    [ObjectSystem]
    public abstract class AwakeSystem<T, A, B, C, D> : IAwakeSystem<A, B, C, D> where T : Entity, IAwake<A, B, C, D>
    {
        // ��ȡϵͳ��Ӧ������
        Type ISystemType.GetType()
        {
            return typeof(T);
        }

        // ��ȡϵͳ����
        Type ISystemType.GetSystemType()
        {
            return typeof(IAwakeSystem<A, B, C, D>);
        }

        // ��ȡʵ�����е�����
        InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
        {
            return InstanceQueueIndex.None;
        }

        // ����Awakeϵͳ
        void IAwakeSystem<A, B, C, D>.Run(Entity o, A a, B b, C c, D d)
        {
            this.Awake((T)o, a, b, c, d);
        }

        // ������һ�������Awake��������Ҫ��������ʵ��
        protected abstract void Awake(T self, A a, B b, C c, D d);
    }
}
