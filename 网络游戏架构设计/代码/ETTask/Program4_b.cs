using System;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    public static class Program4_b
    {
        public static void Main4b()
        {
            Entry.Init();
            Init.Start();
            
            ETCancellationToken cancellationToken = new ETCancellationToken();
            StartCoroutine1(cancellationToken).Coroutine();
            StartCoroutine2(cancellationToken).Coroutine();
            StartCoroutine3(cancellationToken).Coroutine();
            
            
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

        public static async ETTask StartCoroutine1(ETCancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("StartCoroutine1 start");
                await TimerComponent.Instance.WaitAsync(10000, cancellationToken);
                if (cancellationToken.IsCancel())
                {
                    Console.WriteLine("StartCoroutine1 cancel");
                    return;
                }
                Console.WriteLine("StartCoroutine1 finish");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public static async ETTask StartCoroutine2(ETCancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("StartCoroutine2 start");
                await TimerComponent.Instance.WaitAsync(10000, cancellationToken);
                if (cancellationToken.IsCancel())
                {
                    Console.WriteLine("StartCoroutine2 cancel");
                    return;
                }
                Console.WriteLine("StartCoroutine2 finish");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public static async ETTask StartCoroutine3(ETCancellationToken cancellationTokenSource)
        {
            try
            {
                Console.WriteLine($"StartCoroutine3 start {Thread.CurrentThread.ManagedThreadId}");
                await TimerComponent.Instance.WaitAsync(5000);
                cancellationTokenSource.Cancel();
                Console.WriteLine($"StartCoroutine3 finish {Thread.CurrentThread.ManagedThreadId}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}