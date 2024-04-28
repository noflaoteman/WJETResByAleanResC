using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    // 使用ETTask自己实现一个Task.Run(), 判断同步上下文
    public static class Program9
    {
        public static void Main9()
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");

            Game.AddSingleton<MainThreadSynchronizationContext>();
            
            StartTask().Coroutine();

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

        public static async ETTask Run(Action action)
        {
            ETTask tcs = ETTask.Create();
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem((_) =>
            {
                action.Invoke();
                if (synchronizationContext == null)
                {
                    Console.WriteLine($"thread finish callbackaaa: {Thread.CurrentThread.ManagedThreadId}");
                    tcs.SetResult();
                }
                else
                {
                    synchronizationContext.Post((_) => { tcs.SetResult(); }, null);
                }
            });
            await tcs;
        }

        public static async ETTask StartTask()
        {
            await Run(FindPath);

            SendMessage();
        }

        public static void FindPath()
        {
            Console.WriteLine($"FindPath {Thread.CurrentThread.ManagedThreadId}");
        }
        
        public static void SendMessage()
        {
            Console.WriteLine($"SendMessage: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}