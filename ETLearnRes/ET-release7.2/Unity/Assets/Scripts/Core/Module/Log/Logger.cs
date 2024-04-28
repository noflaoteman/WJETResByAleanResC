using System;
using System.Diagnostics;

namespace ET
{
    // 日志记录器类，继承自Singleton<Logger>单例类
    public class Logger : Singleton<Logger>
    {
        // 日志接口，用于实际记录日志
        private ILog iLog;

        // 设置日志接口
        public ILog ILog
        {
            set
            {
                this.iLog = value;
            }
        }

        // 日志级别常量
        private const int TraceLevel = 1;
        private const int DebugLevel = 2;
        private const int InfoLevel = 3;
        private const int WarningLevel = 4;

        // 检查日志级别是否满足要求
        private bool CheckLogLevel(int level)
        {
            // 如果Options实例不存在，说明日志级别未设置，直接返回true
            if (Options.Instance == null)
            {
                return true;
            }
            // 检查Options中的日志级别是否满足要求
            return Options.Instance.LogLevel <= level;
        }

        // 记录跟踪信息
        public void Trace(string msg)
        {
            if (!CheckLogLevel(DebugLevel))
            {
                return;
            }
            StackTrace st = new StackTrace(2, true);
            this.iLog.Trace($"{msg}\n{st}");
        }

        // 记录调试信息
        public void Debug(string msg)
        {
            if (!CheckLogLevel(DebugLevel))
            {
                return;
            }
            this.iLog.Debug(msg);
        }

        // 记录普通信息
        public void Info(string msg)
        {
            if (!CheckLogLevel(InfoLevel))
            {
                return;
            }
            this.iLog.Info(msg);
        }

        // 记录跟踪信息（带信息）
        public void TraceInfo(string msg)
        {
            if (!CheckLogLevel(InfoLevel))
            {
                return;
            }
            StackTrace st = new StackTrace(2, true);
            this.iLog.Trace($"{msg}\n{st}");
        }

        // 记录警告信息
        public void Warning(string msg)
        {
            if (!CheckLogLevel(WarningLevel))
            {
                return;
            }

            this.iLog.Warning(msg);
        }

        // 记录错误信息（带异常）
        public void Error(Exception e)
        {
            if (e.Data.Contains("StackTrace"))
            {
                this.iLog.Error($"{e.Data["StackTrace"]}\n{e}");
                return;
            }
            string str = e.ToString();
            this.iLog.Error(str);
        }

        // 记录跟踪信息（带参数）
        public void Trace(string message, params object[] args)
        {
            if (!CheckLogLevel(TraceLevel))
            {
                return;
            }
            StackTrace st = new StackTrace(2, true);
            this.iLog.Trace($"{string.Format(message, args)}\n{st}");
        }

        // 记录警告信息（带参数）
        public void Warning(string message, params object[] args)
        {
            if (!CheckLogLevel(WarningLevel))
            {
                return;
            }
            this.iLog.Warning(string.Format(message, args));
        }

        // 记录普通信息（带参数）
        public void Info(string message, params object[] args)
        {
            if (!CheckLogLevel(InfoLevel))
            {
                return;
            }
            this.iLog.Info(string.Format(message, args));
        }

        // 记录调试信息（带参数）
        public void Debug(string message, params object[] args)
        {
            if (!CheckLogLevel(DebugLevel))
            {
                return;
            }
            this.iLog.Debug(string.Format(message, args));

        }

        // 记录错误信息（带参数）
        public void Error(string message, params object[] args)
        {
            StackTrace st = new StackTrace(2, true);
            string s = string.Format(message, args) + '\n' + st;
            this.iLog.Error(s);
        }

        // 输出信息到控制台
        public void Console(string message)
        {
            // 如果开启了控制台日志
            if (Options.Instance.Console == 1)
            {
                System.Console.WriteLine(message);
            }
            // 记录调试信息
            this.iLog.Debug(message);
        }

        // 输出信息到控制台（带参数）
        public void Console(string message, params object[] args)
        {
            string s = string.Format(message, args);
            // 如果开启了控制台日志
            if (Options.Instance.Console == 1)
            {
                System.Console.WriteLine(s);
            }
            // 记录调试信息
            this.iLog.Debug(s);
        }
    }
}
