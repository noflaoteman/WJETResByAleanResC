using System;
using System.Collections.Generic;

namespace ET
{
    // ET框架的核心类，负责管理各种单例对象和游戏逻辑的更新
    public static class Game
    {
        #region readonly 的解释
        /*字段都被声明为 readonly，这意味着它们的值只能在构造函数中赋值一次，之后不能再被修改*/
        #endregion
        // 存储单例对象类型与实例的字典
        private static readonly Dictionary<Type, ISingleton> singletonTypes = new Dictionary<Type, ISingleton>();
        // 存储所有单例对象的栈，用于顺序销毁 (为了相反的顺序销毁)
        private static readonly Stack<ISingleton> singletons = new Stack<ISingleton>();
        // 存储需要每帧更新的单例对象队列
        private static readonly Queue<ISingleton> updates = new Queue<ISingleton>();
        // 存储需要每帧LateUpdate的单例对象队列
        private static readonly Queue<ISingleton> lateUpdates = new Queue<ISingleton>();
        // 存储每帧结束时需要执行的任务队列
        private static readonly Queue<ETTask> frameFinishTask = new Queue<ETTask>();

        // 添加一个单例对象并返回该对象的实例
        public static T AddSingleton<T>() where T : Singleton<T>, new()
        {
            T singleton = new T();
            AddSingleton(singleton);
            return singleton;
        }

        // 添加一个单例对象
        public static void AddSingleton(ISingleton singleton)
        {
            Type singletonType = singleton.GetType();
            if (singletonTypes.ContainsKey(singletonType))
            {
                throw new Exception($"already exist singleton: {singletonType.Name}");
            }

            singletonTypes.Add(singletonType, singleton);
            singletons.Push(singleton);

            // 调用单例对象的注册方法
            singleton.Register();

            // 如果单例对象实现了ISingletonAwake接口，则调用其Awake方法
            if (singleton is ISingletonAwake awake)
            {
                awake.Awake();
            }

            // 如果单例对象实现了ISingletonUpdate接口，则加入更新队列
            if (singleton is ISingletonUpdate)
            {
                updates.Enqueue(singleton);
            }

            // 如果单例对象实现了ISingletonLateUpdate接口，则加入LateUpdate队列
            if (singleton is ISingletonLateUpdate)
            {
                lateUpdates.Enqueue(singleton);
            }
        }

        // 等待当前帧结束
        public static async ETTask WaitFrameFinish()
        {
            ETTask task = ETTask.Create(true);
            frameFinishTask.Enqueue(task);
            await task;
        }

        // 每帧更新游戏逻辑
        public static void Update()
        {
            int count = updates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = updates.Dequeue();

                if (singleton.IsDisposed())
                {
                    continue;
                }

                if (singleton is not ISingletonUpdate update)
                {
                    continue;
                }

                updates.Enqueue(singleton);
                try
                {
                    update.Update();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        // 每帧LateUpdate
        public static void LateUpdate()
        {
            int count = lateUpdates.Count;
            while (count-- > 0)
            {
                ISingleton singleton = lateUpdates.Dequeue();

                if (singleton.IsDisposed())
                {
                    continue;
                }

                if (singleton is not ISingletonLateUpdate lateUpdate)
                {
                    continue;
                }

                lateUpdates.Enqueue(singleton);
                try
                {
                    lateUpdate.LateUpdate();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        // 帧结束后执行任务
        public static void FrameFinishUpdate()
        {
            while (frameFinishTask.Count > 0)
            {
                ETTask task = frameFinishTask.Dequeue();
                task.SetResult();
            }
        }

        // 关闭游戏
        public static void Close()
        {
            // 逆序清理单例对象
            while (singletons.Count > 0)
            {
                ISingleton iSingleton = singletons.Pop();
                iSingleton.Destroy();
            }
            singletonTypes.Clear();
        }
    }
}
