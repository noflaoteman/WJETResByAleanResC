using System;

namespace ET
{
    /// <summary>
    /// 定义一个单例接口，继承自 IDisposable 接口
    /// </summary>
    public interface ISingleton : IDisposable
    {
        /// <summary>
        /// 注册单例
        /// </summary>
        void Register();

        /// <summary>
        /// 销毁单例
        /// </summary>
        void Destroy();

        /// <summary>
        /// 检查单例是否已释放
        /// </summary>
        /// <returns>如果单例已释放，返回 true，否则返回 false</returns>
        bool IsDisposed();
    }

    /// <summary>
    /// 泛型单例基类，实现了 ISingleton 接口
    /// </summary>
    /// <typeparam name="T">派生类的类型</typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private bool isDisposed; // 标记单例是否已释放
        [StaticField] // 静态字段标记，用于某些 AOT 编译器的支持
        private static T instance; // 单例实例

        /// <summary>
        /// 获取单例实例的静态属性
        /// </summary>
        public static T Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        void ISingleton.Register()
        {
            // 检查是否已经注册过单例
            if (instance != null)
            {
                throw new Exception($"singleton register twice! {typeof(T).Name}");
            }
            instance = (T)this; // 注册单例
        }

        /// <summary>
        /// 销毁单例
        /// </summary>
        void ISingleton.Destroy()
        {
            if (this.isDisposed) // 如果已经释放过单例，则直接返回
            {
                return;
            }
            this.isDisposed = true; // 标记为已释放

            instance.Dispose(); // 调用单例的 Dispose 方法释放资源
            instance = null; // 将单例实例置空
        }

        /// <summary>
        /// 检查单例是否已释放
        /// </summary>
        /// <returns>如果单例已释放，返回 true，否则返回 false</returns>
        bool ISingleton.IsDisposed()
        {
            return this.isDisposed;
        }

        /// <summary>
        /// 实现 IDisposable 接口的 Dispose 方法，用于释放资源
        /// </summary>
        public virtual void Dispose()
        {
            // 子类如果需要释放资源，可以在此方法中实现
        }
    }
}
