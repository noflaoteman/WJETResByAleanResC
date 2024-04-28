using System;
using System.Threading;

namespace ET
{
    // 线程结果回调
    public static class Program3
    {
        public static void Main3()
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");

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
            Thread thread = new Thread((_) => { action.Invoke(); });
            thread.Start();
        }

        public static void FindPath(Action threadFinishCallback)
        {
            Console.WriteLine($"FindPath {Thread.CurrentThread.ManagedThreadId}");
            
            threadFinishCallback.Invoke();
        }

        public static void SendMessage()
        {
            Console.WriteLine($"SendMessage: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}