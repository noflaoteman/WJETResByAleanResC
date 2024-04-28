using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    public static class Program3
    {
        public static void Main3()
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

        public static async void StartCoroutine()
        {
            await FindPath();
            SendMessage();
        }

        public static async Task FindPath()
        {
            TaskCompletionSource tcs = new TaskCompletionSource();
            ThreadPool.QueueUserWorkItem(state =>
            {
                Console.WriteLine("FindPath run!");
                tcs.SetResult();
            });
            await tcs.Task;
        }
        
        public static void SendMessage()
        {
            Console.WriteLine("SendMessage run!");
        }
    }
}