using System;

namespace ET
{
    // 时间信息类，继承自Singleton<TimeInfo>单例类，并实现ISingletonUpdate接口
    public class TimeInfo : Singleton<TimeInfo>, ISingletonUpdate
    {
        // 时区
        private int timeZone;

        // 获取或设置时区
        public int TimeZone
        {
            get
            {
                return this.timeZone;
            }
            set
            {
                // 设置时区并更新时间
                this.timeZone = value;
                dt = dt1970.AddHours(TimeZone);
            }
        }

        // 1970年的日期时间
        private DateTime dt1970;
        // 当前日期时间
        private DateTime dt;

        // 服务器与客户端时间差
        public long ServerMinusClientTime { private get; set; }

        // 当前帧的时间
        public long FrameTime;

        // 构造函数，初始化时间信息
        public TimeInfo()
        {
            // 初始化1970年的日期时间
            this.dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // 初始化当前日期时间
            this.dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // 初始化当前帧的时间
            this.FrameTime = this.ClientNow();
        }

        // 实现ISingletonUpdate接口的方法，用于每帧更新时间
        public void Update()
        {
            // 更新当前帧的时间
            this.FrameTime = this.ClientNow();
        }

        /// <summary> 
        /// 根据时间戳获取日期时间 
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }

        // 获取当前客户端时间的时间戳（毫秒），线程安全
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - this.dt1970.Ticks) / 10000;
        }

        // 获取当前服务器时间的时间戳（毫秒）
        public long ServerNow()
        {
            return ClientNow() + Instance.ServerMinusClientTime;
        }

        // 获取当前帧的客户端时间的时间戳（毫秒）
        public long ClientFrameTime()
        {
            return this.FrameTime;
        }

        // 获取当前帧的服务器时间的时间戳（毫秒）
        public long ServerFrameTime()
        {
            return this.FrameTime + Instance.ServerMinusClientTime;
        }

        // 获取指定日期时间与初始时间的时间戳（毫秒）
        public long Transition(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 10000;
        }
    }
}
