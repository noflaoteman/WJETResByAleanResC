using System;
using System.Collections.Generic;
using ET;

namespace Lesson12_01;


public abstract class BaseMessage
{
    public abstract void Handle();
}

public class C2M_SendToServer: BaseMessage
{
    public override void Handle()
    {
        
    }
}







public static class MethodDispatcher
{
    public static void HandleMessage1(int opcode, object message)
    {
        switch (opcode)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    private static Dictionary<int, Action<int, object>> messageHandler = new Dictionary<int, Action<int, object>>();

    static MethodDispatcher()
    {
        messageHandler.Add(1, (opcode, message) =>
        {
            Console.WriteLine($"111111111 {opcode}");
        });
        
        messageHandler.Add(2, (opcode, message) =>
        {
            Console.WriteLine($"111111112 {opcode}");
        });
        
        messageHandler.Add(3, (opcode, message) =>
        {
            Console.WriteLine($"111111113 {opcode}");
        });
    }

    public static void HandleMessage2(int opcode, object message)
    {
        if (!messageHandler.TryGetValue(opcode, out Action<int, object> action))
        {
            return;
        }
        action.Invoke(opcode, message);
    }
}
