using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    // 自己实现一个Task.Run()
    public static class Program8
    {
        public static void Main8()
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

        public static async Task Run(Action action)
        {
            TaskCompletionSource tcs = new TaskCompletionSource();
            ThreadPool.QueueUserWorkItem((_) =>
            {
                action.Invoke();
                Console.WriteLine($"set result: {Thread.CurrentThread.ManagedThreadId}");
                tcs.SetResult();
            });
            await tcs.Task;
        }

        public static async void StartTask()
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