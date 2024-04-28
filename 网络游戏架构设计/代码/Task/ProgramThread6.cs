using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    // await Task到主线程
    public static class Program7
    {
        public static void Main7()
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");

            Game.AddSingleton<MainThreadSynchronizationContext>();
            
            StartTask();

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

        public static async void StartTask()
        {
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;
            await Task.Run(FindPath);
            
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