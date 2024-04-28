using System;
using System.Threading;

namespace ET
{
    public static class Program5
    {
        public static void Main5()
        {
            //Entry.Init();
            //Init.Start();
            
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

            await ETTask.CompletedTask;
            
            Console.WriteLine("AAAAAAAAA finish");
        }
    }
}