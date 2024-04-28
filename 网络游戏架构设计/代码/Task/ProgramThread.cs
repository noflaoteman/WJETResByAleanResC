using System;
using System.Threading;

namespace ET
{
    // 线程举例
    public static class Program2
    {
        public static void Main2()
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");
            
            Thread thread = new Thread(
                ()=>{Console.WriteLine($"thread start {Thread.CurrentThread.ManagedThreadId}");});
            
            Console.WriteLine($"main thread2 {Thread.CurrentThread.ManagedThreadId}");
            thread.Start();
            thread.Join();
            Console.WriteLine($"main thread3 {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}