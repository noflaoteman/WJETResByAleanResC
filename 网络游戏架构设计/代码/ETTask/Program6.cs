using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using ET;

public static class Program6
{
    // Methods
    public static ETTask AAAAAAAAA()
    {
        AAAAAAAAAd__1 d__;
        d__.pt__builder = ETAsyncTaskMethodBuilder.Create();
        d__.p1__state = -1;
        d__.pt__builder.Start<AAAAAAAAAd__1>(ref d__);
        return d__.pt__builder.Task;
    }

    public static void Main6()
    {
        AAAAAAAAA().Coroutine();
    Label_0014:
        Thread.Sleep(1);
        try
        {
            Init.Update();
            Init.LateUpdate();
            Init.FrameFinishUpdate();
            goto Label_0014;
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            goto Label_0014;
        }
    }

    // Nested Types
    private struct AAAAAAAAAd__1 : IAsyncStateMachine
    {
        // Fields
        public int p1__state;
        public ETAsyncTaskMethodBuilder pt__builder;
        private ETTaskCompleted pu__1;

        // Methods
        public void MoveNext()
        {
            int num = this.p1__state;
            try
            {
                ETTaskCompleted awaiter;
                if (num != 0)
                {
                    Console.WriteLine("AAAAAAAAA start");
                    awaiter = ETTask.CompletedTask.GetAwaiter();
                    if (!awaiter.IsCompleted)
                    {
                        this.p1__state = num = 0;
                        this.pu__1 = awaiter;
                        this.pt__builder.AwaitUnsafeOnCompleted<ETTaskCompleted, Program6.AAAAAAAAAd__1>(ref awaiter, ref this);
                        return;
                    }
                }
                else
                {
                    awaiter = this.pu__1;
                    this.pu__1 = new ETTaskCompleted();
                    this.p1__state = num = -1;
                }
                awaiter.GetResult();
                Console.WriteLine("AAAAAAAAA finish");
            }
            catch (Exception exception)
            {
                this.p1__state = -2;
                this.pt__builder.SetException(exception);
                return;
            }
            this.p1__state = -2;
            this.pt__builder.SetResult();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            this.pt__builder.SetStateMachine(stateMachine);
        }
    }
}
 
