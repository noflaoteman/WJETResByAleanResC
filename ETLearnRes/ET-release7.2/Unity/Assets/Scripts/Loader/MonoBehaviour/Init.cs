using System;
using System.Threading;
using CommandLine; // 引入命令行解析库
using UnityEngine; // 引入Unity引擎的命名空间

namespace ET
{
    // ET框架的入口类，继承自Unity的MonoBehaviour类
    public class Init : MonoBehaviour
    {
        // 在游戏启动时调用的方法
        private void Start()
        {
            // 将当前的gameObject设置为不会在场景切换时被销毁
            DontDestroyOnLoad(gameObject);

            // 注册全局异常处理事件
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                // 将异常信息记录到日志中
                Log.Error(e.ExceptionObject.ToString());
            };



            // 命令行参数
            // 在这里你需要将命令行参数传递给args变量，这里暂时为空字符串数组
            string[] args = "".Split(" ");
            // 解析命令行参数，并将解析结果添加到Game类中
            Parser.Default.ParseArguments<Options>(args)
                .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                .WithParsed(Game.AddSingleton);


            // 添加各种组件到Game类中
            // 向Game类中添加MainThreadSynchronizationContext组件
            Game.AddSingleton<MainThreadSynchronizationContext>();
            // 注册TimeInfo组件到Game类
            Game.AddSingleton<TimeInfo>();
            // 注册Logger组件到Game类，并设置ILog属性为UnityLogger的实例
            Game.AddSingleton<Logger>().ILog = new UnityLogger();
            // 注册ObjectPool组件到Game类
            Game.AddSingleton<ObjectPool>();
            // 注册IdGenerater组件到Game类
            Game.AddSingleton<IdGenerater>();
            // 注册EventSystem组件到Game类
            Game.AddSingleton<EventSystem>();
            // 注册TimerComponent组件到Game类
            Game.AddSingleton<TimerComponent>();
            // 注册CoroutineLockComponent组件到Game类
            Game.AddSingleton<CoroutineLockComponent>();
            // 向Game类中添加CodeLoader组件，并启动它
            Game.AddSingleton<CodeLoader>().Start();


            // 设置ETTask异常处理器
            ETTask.ExceptionHandler += Log.Error;

        }

        // 在每一帧更新时调用的方法
        private void Update()
        {
            // 更新游戏逻辑
            Game.Update();
        }

        // 在每一帧LateUpdate时调用的方法
        private void LateUpdate()
        {
            // 在LateUpdate中处理一些延迟更新的任务
            Game.LateUpdate();
            Game.FrameFinishUpdate();
        }

        // 当应用程序退出时调用的方法
        private void OnApplicationQuit()
        {
            // 关闭游戏
            Game.Close();
        }
    }
}
