using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ET
{
    // 自定义Task调度器
    public static class Program10
    {
        public static void Main()        
        {
            Console.WriteLine($"main thread {Thread.CurrentThread.ManagedThreadId}");

            //Game.AddSingleton<MainThreadSynchronizationContext>();
            
            StartTask();

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

        public static async void StartTask()
        {
            await Task.Factory.StartNew(FindPath, new CancellationToken(), TaskCreationOptions.None, new MyTaskScheduler());

            SendMessage();
        }

        public static void FindPath()
        {
            Console.WriteLine($"FindPath {Thread.CurrentThread.ManagedThreadId}");
        }
        
        public static void SendMessage()
        {
            Console.WriteLine($"SendMessage: {Thread.CurrentThread.ManagedThreadId}");
        }

        public class MyTaskScheduler: TaskScheduler
        {
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return null;
            }

            protected override void QueueTask(Task task)
            {
                TryExecuteTask(task);
                
                //ThreadPool.QueueUserWorkItem((_)=>TryExecuteTask(task));
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                throw new NotImplementedException();
            }
        }
    }
}