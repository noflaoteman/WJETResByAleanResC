public static class Program
{
    // Methods
    public static void Main()
    {
        Entry.Init();
        Init.Start();
        Test111111().Coroutine();
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

    [AsyncStateMachine((Type) typeof(<Test111111>d__1))]
    public static ETTask Test111111()
    {
        <Test111111>d__1 d__;
        d__.<>t__builder = ETAsyncTaskMethodBuilder.Create();
        d__.<>1__state = -1;
        d__.<>t__builder.Start<<Test111111>d__1>(ref d__);
        return d__.<>t__builder.Task;
    }

    // Nested Types
    [CompilerGenerated]
    private struct <Test111111>d__1 : IAsyncStateMachine
    {
        // Fields
        public int <>1__state;
        public ETAsyncTaskMethodBuilder <>t__builder;
        private ETTaskCompleted <>u__1;

        // Methods
        private void MoveNext()
        {
            int num = this.<>1__state;
            try
            {
                ETTaskCompleted awaiter;
                if (num != 0)
                {
                    awaiter = ETTask.CompletedTask.GetAwaiter();
                    if (!awaiter.IsCompleted)
                    {
                        this.<>1__state = num = 0;
                        this.<>u__1 = awaiter;
                        this.<>t__builder.AwaitUnsafeOnCompleted<ETTaskCompleted, Program.<Test111111>d__1>(ref awaiter, ref this);
                        return;
                    }
                }
                else
                {
                    awaiter = this.<>u__1;
                    this.<>u__1 = new ETTaskCompleted();
                    this.<>1__state = num = -1;
                }
                awaiter.GetResult();
            }
            catch (Exception exception)
            {
                this.<>1__state = -2;
                this.<>t__builder.SetException(exception);
                return;
            }
            this.<>1__state = -2;
            this.<>t__builder.SetResult();
        }

        [DebuggerHidden]
        private void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            this.<>t__builder.SetStateMachine(stateMachine);
        }
    }
}

 
