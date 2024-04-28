using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    public static class Program4
    {
        public static void Main4()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            StartCoroutine1(cancellationTokenSource.Token);
            StartCoroutine2(cancellationTokenSource);
            
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

        public static async void StartCoroutine1(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("StartCoroutine1 start");
                await Task.Delay(10000, cancellationToken);
                Console.WriteLine("StartCoroutine1 finish");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public static async void StartCoroutine2(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                Console.WriteLine($"StartCoroutine2 start {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(5000);
                cancellationTokenSource.Cancel();
                Console.WriteLine($"StartCoroutine2 finish {Thread.CurrentThread.ManagedThreadId}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}