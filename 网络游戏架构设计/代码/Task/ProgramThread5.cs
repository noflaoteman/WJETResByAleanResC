using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    // await Task, 相当于线程结果回调
    public static class Program6
    {
        public static void Main6()
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");

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