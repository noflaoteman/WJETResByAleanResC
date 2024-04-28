using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 协程锁组件，用于管理协程锁和异步等待
    /// </summary>
    public class CoroutineLockComponent : Singleton<CoroutineLockComponent>, ISingletonUpdate
    {
        // 存储每种协程锁的队列
        private readonly List<CoroutineLockQueueType> list = new List<CoroutineLockQueueType>(CoroutineLockType.Max);

        // 存储下一帧要执行的协程信息队列
        private readonly Queue<(int, long, int)> nextFrameRun = new Queue<(int, long, int)>();

        /// <summary>
        /// 构造函数，初始化协程锁队列
        /// </summary>
        public CoroutineLockComponent()
        {
            for (int i = 0; i < CoroutineLockType.Max; ++i)
            {
                CoroutineLockQueueType coroutineLockQueueType = new CoroutineLockQueueType(i);
                this.list.Add(coroutineLockQueueType);
            }
        }

        /// <summary>
        /// 释放资源，清空队列
        /// </summary>
        public override void Dispose()
        {
            this.list.Clear();
            this.nextFrameRun.Clear();
        }

        /// <summary>
        /// 每帧更新，处理下一帧要执行的协程信息
        /// </summary>
        public void Update()
        {
            // 循环过程中会有对象继续加入队列
            while (this.nextFrameRun.Count > 0)
            {
                (int coroutineLockType, long key, int count) = this.nextFrameRun.Dequeue();
                this.Notify(coroutineLockType, key, count);
            }
        }

        /// <summary>
        /// 将下一帧要执行的协程加入队列
        /// </summary>
        /// <param name="coroutineLockType">协程锁类型</param>
        /// <param name="key">协程锁的Key</param>
        /// <param name="level">协程等级</param>
        public void RunNextCoroutine(int coroutineLockType, long key, int level)
        {
            // 一个协程队列一帧处理超过100个,说明比较多了,打个warning,检查一下是否够正常
            if (level == 100)
            {
                Log.Warning($"too much coroutine level: {coroutineLockType} {key}");
            }

            this.nextFrameRun.Enqueue((coroutineLockType, key, level));
        }

        /// <summary>
        /// 异步等待协程锁
        /// </summary>
        /// <param name="coroutineLockType">协程锁类型</param>
        /// <param name="key">协程锁的Key</param>
        /// <param name="time">超时时间，默认60秒</param>
        /// <returns>返回一个异步操作，用于等待协程锁释放</returns>
        public async ETTask<CoroutineLock> Wait(int coroutineLockType, long key, int time = 60000)
        {
            CoroutineLockQueueType coroutineLockQueueType = this.list[coroutineLockType];
            return await coroutineLockQueueType.Wait(key, time);
        }

        /// <summary>
        /// 通知协程锁释放
        /// </summary>
        /// <param name="coroutineLockType">协程锁类型</param>
        /// <param name="key">协程锁的Key</param>
        /// <param name="level">协程等级</param>
        private void Notify(int coroutineLockType, long key, int level)
        {
            CoroutineLockQueueType coroutineLockQueueType = this.list[coroutineLockType];
            coroutineLockQueueType.Notify(key, level);
        }
    }
}
