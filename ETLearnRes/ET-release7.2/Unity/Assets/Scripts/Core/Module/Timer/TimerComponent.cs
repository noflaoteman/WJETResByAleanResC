using System.Collections.Generic;

namespace ET
{
    // 定义计时器类型枚举
    public enum TimerClass
    {
        None,            // 无
        OnceTimer,       // 一次性计时器
        OnceWaitTimer,   // 一次性等待计时器
        RepeatedTimer,   // 重复计时器
    }

    // 计时器动作类
    public class TimerAction
    {
        // 创建计时器动作实例
        public static TimerAction Create(long id, TimerClass timerClass, long startTime, long time, int type, object obj)
        {
            TimerAction timerAction = ObjectPool.Instance.Fetch<TimerAction>();  // 从对象池获取实例
            timerAction.Id = id;
            timerAction.TimerClass = timerClass;
            timerAction.StartTime = startTime;
            timerAction.Object = obj;
            timerAction.Time = time;
            timerAction.Type = type;
            return timerAction;
        }

        public long Id;             // 计时器ID
        public TimerClass TimerClass;  // 计时器类型
        public object Object;          // 附加对象
        public long StartTime;         // 开始时间
        public long Time;              // 时间
        public int Type;               // 类型

        // 回收计时器动作
        public void Recycle()
        {
            this.Id = 0;
            this.Object = null;
            this.StartTime = 0;
            this.Time = 0;
            this.TimerClass = TimerClass.None;
            this.Type = 0;
            ObjectPool.Instance.Recycle(this);  // 回收实例到对象池
        }
    }

    // 计时器回调结构
    public struct TimerCallback
    {
        public object Args;  // 参数
    }

    // 计时器组件类
    public class TimerComponent : Singleton<TimerComponent>, ISingletonUpdate
    {
        // 时间与计时器ID的多对多映射
        private readonly MultiMap<long, long> TimeId = new();

        // 超时时间队列
        private readonly Queue<long> timeOutTime = new();

        // 超时计时器ID队列
        private readonly Queue<long> timeOutTimerIds = new();

        // 计时器动作字典
        private readonly Dictionary<long, TimerAction> timerActions = new();

        private long idGenerator;  // ID生成器

        // 最小时间，用于优化查找
        private long minTime = long.MaxValue;

        // 获取新的ID
        private long GetId()
        {
            return ++this.idGenerator;
        }

        // 获取当前时间
        private static long GetNow()
        {
            return TimeHelper.ClientFrameTime();
        }

        // 更新函数
        public void Update()
        {
            if (this.TimeId.Count == 0)
            {
                return;
            }

            long timeNow = GetNow();

            if (timeNow < this.minTime)
            {
                return;
            }

            // 检查超时时间并加入队列
            foreach (KeyValuePair<long, List<long>> kv in this.TimeId)
            {
                long k = kv.Key;
                if (k > timeNow)
                {
                    this.minTime = k;
                    break;
                }

                this.timeOutTime.Enqueue(k);
            }

            // 处理超时事件
            while (this.timeOutTime.Count > 0)
            {
                long time = this.timeOutTime.Dequeue();
                var list = this.TimeId[time];
                for (int i = 0; i < list.Count; ++i)
                {
                    long timerId = list[i];
                    this.timeOutTimerIds.Enqueue(timerId);
                }
                this.TimeId.Remove(time);
            }

            // 处理超时计时器
            while (this.timeOutTimerIds.Count > 0)
            {
                long timerId = this.timeOutTimerIds.Dequeue();

                if (!this.timerActions.Remove(timerId, out TimerAction timerAction))
                {
                    continue;
                }

                this.Run(timerAction);
            }
        }

        // 处理计时器动作
        private void Run(TimerAction timerAction)
        {
            switch (timerAction.TimerClass)
            {
                case TimerClass.OnceTimer:
                    {
                        EventSystem.Instance.Invoke(timerAction.Type, new TimerCallback() { Args = timerAction.Object });
                        timerAction.Recycle();
                        break;
                    }
                case TimerClass.OnceWaitTimer:
                    {
                        ETTask tcs = timerAction.Object as ETTask;
                        tcs.SetResult();
                        timerAction.Recycle();
                        break;
                    }
                case TimerClass.RepeatedTimer:
                    {
                        long timeNow = GetNow();
                        timerAction.StartTime = timeNow;
                        this.AddTimer(timerAction);
                        EventSystem.Instance.Invoke(timerAction.Type, new TimerCallback() { Args = timerAction.Object });
                        break;
                    }
            }
        }

        // 添加计时器
        private void AddTimer(TimerAction timer)
        {
            long tillTime = timer.StartTime + timer.Time;
            this.TimeId.Add(tillTime, timer.Id);
            this.timerActions.Add(timer.Id, timer);
            if (tillTime < this.minTime)
            {
                this.minTime = tillTime;
            }
        }

        // 移除计时器
        public bool Remove(ref long id)
        {
            long i = id;
            id = 0;
            return this.Remove(i);
        }

        // 移除计时器
        private bool Remove(long id)
        {
            if (id == 0)
            {
                return false;
            }

            if (!this.timerActions.Remove(id, out TimerAction timerAction))
            {
                return false;
            }
            timerAction.Recycle();
            return true;
        }

        // 异步等待到指定时间
        public async ETTask WaitTillAsync(long tillTime, ETCancellationToken cancellationToken = null)
        {
            long timeNow = GetNow();
            if (timeNow >= tillTime)
            {
                return;
            }

            ETTask tcs = ETTask.Create(true);
            TimerAction timer = TimerAction.Create(this.GetId(), TimerClass.OnceWaitTimer, timeNow, tillTime - timeNow, 0, tcs);
            this.AddTimer(timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (this.Remove(timerId))
                {
                    tcs.SetResult();
                }
            }

            try
            {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
        }

        // 异步等待一帧
        public async ETTask WaitFrameAsync(ETCancellationToken cancellationToken = null)
        {
            await this.WaitAsync(1, cancellationToken);
        }

        // 异步等待指定时间
        public async ETTask WaitAsync(long time, ETCancellationToken cancellationToken = null)
        {
            if (time == 0)
            {
                return;
            }

            long timeNow = GetNow();

            ETTask tcs = ETTask.Create(true);
            TimerAction timer = TimerAction.Create(this.GetId(), TimerClass.OnceWaitTimer, timeNow, time, 0, tcs);
            this.AddTimer(timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (this.Remove(timerId))
                {
                    tcs.SetResult();
                }
            }

            try
            {
                cancellationToken?.Add(CancelAction);
                await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
        }

        // 创建一次性计时器并返回ID
        public long NewOnceTimer(long tillTime, int type, object args)
        {
            long timeNow = GetNow();
            if (tillTime < timeNow)
            {
                Log.Error($"new once time too small: {tillTime}");
            }

            TimerAction timer = TimerAction.Create(this.GetId(), TimerClass.OnceTimer, timeNow, tillTime - timeNow, type, args);
            this.AddTimer(timer);
            return timer.Id;
        }

        // 创建一帧计时器并返回ID
        public long NewFrameTimer(int type, object args)
        {
#if DOTNET
            return this.NewRepeatedTimerInner(100, type, args);
#else
            return this.NewRepeatedTimerInner(0, type, args);
#endif
        }

        // 创建重复计时器并返回ID
        private long NewRepeatedTimerInner(long time, int type, object args)
        {
#if DOTNET
            if (time < 100)
            {
                throw new Exception($"repeated timer < 100, timerType: time: {time}");
            }
#endif

            long timeNow = GetNow();
            TimerAction timer = TimerAction.Create(this.GetId(), TimerClass.RepeatedTimer, timeNow, time, type, args);
            this.AddTimer(timer);
            return timer.Id;
        }

        // 创建重复计时器并返回ID
        public long NewRepeatedTimer(long time, int type, object args)
        {
            if (time < 100)
            {
                Log.Error($"time too small: {time}");
                return 0;
            }

            return this.NewRepeatedTimerInner(time, type, args);
        }
    }
}
