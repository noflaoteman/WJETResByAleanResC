using System;
using System.Threading;
using CommandLine;

namespace ET
{
    public static class Program
    {
        public static void Main()
        {
            //Entry.Init();
            //Init.Start();
            
            // 命令行参数
            Options options = null;
            Parser.Default.ParseArguments<Options>(System.Environment.GetCommandLineArgs())
                    .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                    .WithParsed((o) => { options = o;});
            
            Console.WriteLine($"command line is: {Parser.Default.FormatCommandLine(options)}");
            
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

        public static async ETTask F(ETCancelToken cancelToken)
        {

        }

        public static async ETTask F2(ETCancelToken cancelToken)
        {
            
        }
    }
}