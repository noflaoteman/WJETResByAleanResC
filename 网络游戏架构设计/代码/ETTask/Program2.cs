using System;
using System.Threading;

namespace ET
{
    public static class Program2
    {
        public static void Main2()
        {
            StartCoroutine();
            
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

        public static void StartCoroutine()
        {
            FindPath(SendMessage);
        }

        public static void FindPath(Action callback)
        {
            ThreadPool.QueueUserWorkItem((_) => {             
                Console.WriteLine("FindPath run!");
                callback.Invoke(); }
                );
        }
        
        public static void SendMessage()
        {
            Console.WriteLine("SendMessage run!");
        }
    }
}