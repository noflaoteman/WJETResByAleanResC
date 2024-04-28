using System;
using System.Threading;

namespace ET
{
    public static class Program7
    {
        public static void Main7()
        {
            Entry.Init();
            Init.Start();
            
            AAAAAAAAA().Coroutine();
            
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

        public static async ETTask AAAAAAAAA()
        {
            Console.WriteLine("AAAAAAAAA start");

            await BBBBBBBBB();
            
            Console.WriteLine("AAAAAAAAA finish");
        }
        
        public static async ETTask BBBBBBBBB()
        {
            Console.WriteLine("BBBBBBBBB start");

            await TimerComponent.Instance.WaitAsync(1000);
            
            Console.WriteLine("BBBBBBBBB finish");
        }
    }
}