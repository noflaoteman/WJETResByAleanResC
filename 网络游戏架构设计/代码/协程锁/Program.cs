using System;
using System.Threading;

namespace ET
{
    public static class Program
    {
        public static void Main()
        {
            Entry.Init();
            Init.Start();

            //Test2().Coroutine();
            //CoroutineLock1();
            CoroutineLockDead().Coroutine();
            
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

        public static async ETTask Test()
        {
            Console.WriteLine("start");
            await TimerComponent.Instance.WaitAsync(5000);
            Console.WriteLine("stop");
        }

        public static async ETTask Test2()
        {
            Console.WriteLine("start");
            for (int i = 0; i < 5; ++i)
            {
                await TimerComponent.Instance.WaitAsync(1000);
                Console.WriteLine($"time {i + 1}");
            }
            Console.WriteLine("stop");
        }
        
        public static void CoroutineLock1()
        {
            CoroutineLock1a().Coroutine();
            CoroutineLock1b().Coroutine();
        }
        
        public static async ETTask CoroutineLock1a()
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Mailbox, 1))
            {
                Console.WriteLine("CoroutineLock1_a start");
                await TimerComponent.Instance.WaitAsync(5000);
                Console.WriteLine("CoroutineLock1_a finish");
            }
        }
        
        public static async ETTask CoroutineLock1b()
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Mailbox, 1))
            {
                Console.WriteLine("CoroutineLock1_b start");
                await TimerComponent.Instance.WaitAsync(10000);
                Console.WriteLine("CoroutineLock1_b finish");
            }
        }
        
        public static async ETTask CoroutineLockDead()
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Mailbox, 1))
            {
                Console.WriteLine("CoroutineLock1_b start");
                
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Mailbox, 1))
                {
                    Console.WriteLine("CoroutineLock1_b start 2");
                    await TimerComponent.Instance.WaitAsync(1000);
                    Console.WriteLine("CoroutineLock1_b finish 2");
                }
                
                Console.WriteLine("CoroutineLock1_b finish");
            }
        }
    }
}