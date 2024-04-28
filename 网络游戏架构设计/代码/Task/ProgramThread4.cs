using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    // Task
    public static class Program5
    {
        public static void Main5()
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");
            
            Game.AddSingleton<MainThreadSynchronizationContext>();
            
            Task.Run(()=>
            {
                FindPath(SendMessage);
            });
            
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