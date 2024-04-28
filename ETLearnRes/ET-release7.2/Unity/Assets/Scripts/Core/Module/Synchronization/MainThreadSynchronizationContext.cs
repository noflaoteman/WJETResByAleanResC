using System;
using System.Threading;

namespace ET
{
    // 主线程同步上下文类，用于在主线程上下文中执行异步操作
    public class MainThreadSynchronizationContext : Singleton<MainThreadSynchronizationContext>, ISingletonUpdate
    {
        // 存储线程同步上下文的实例
        private readonly ThreadSynchronizationContext threadSynchronizationContext = new ThreadSynchronizationContext();

        // 构造函数，初始化主线程同步上下文实例
        public MainThreadSynchronizationContext()
        {
            // 设置当前的同步上下文为线程同步上下文
            SynchronizationContext.SetSynchronizationContext(this.threadSynchronizationContext);
        }

        // 实现ISingletonUpdate接口的方法，用于每帧更新
        public void Update()
        {
            // 更新线程同步上下文
            this.threadSynchronizationContext.Update();
        }

        // 向主线程同步上下文中发布一个异步操作
        public void Post(SendOrPostCallback callback, object state)
        {
            // 将回调函数和状态封装为一个委托，然后发布到主线程同步上下文中执行
            this.Post(() => callback(state));
        }

        // 向主线程同步上下文中发布一个异步操作
        public void Post(Action action)
        {
            // 将操作封装为一个委托，然后发布到主线程同步上下文中执行
            this.threadSynchronizationContext.Post(action);
        }
    }
}
