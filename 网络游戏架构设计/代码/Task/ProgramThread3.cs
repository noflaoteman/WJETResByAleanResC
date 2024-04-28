using System;
using System.Threading;

namespace ET
{
    // 线程结果回调
    public static class Program4
    {
        public static void Main4()
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");
            
            Game.AddSingleton<MainThreadSynchronizationContext>();

            StartThread(()=>FindPath(SendMessage));
            
            while (true)
            {
                Thread.Sleep(1);
                try
                {
                    Init.Update();
                    Init.LateUpdate();
                    Init.FrameFinishUpdate();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static void StartThread(Action action)
        {
            ThreadPool.QueueUserWorkItem((_) => { action.Invoke(); });
        }

        public static void FindPath(Action threadFinishCallback)
        {
            Console.WriteLine($"FindPath {Thread.CurrentThread.ManagedThreadId}");
            
            MainThreadSynchronizationContext.Instance.Post(threadFinishCallback);
        }

        public static void SendMessage()
        {
            Console.WriteLine($"SendMessage: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}