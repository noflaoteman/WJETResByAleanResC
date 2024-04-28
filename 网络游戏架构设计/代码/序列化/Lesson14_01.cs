using System;
using System.Collections.Generic;
using System.IO;
using ET;
using ProtoBuf;
using ProtoBuf.Meta;
using Unity.Mathematics;

namespace Lesson14_01;

[ProtoContract]
[ProtoInclude(5, typeof(BB))]
public class AA
{
    [ProtoMember(1)]
    public int a;

    [ProtoMember(2)]
    public float3 pos;
}

public class BB: AA
{
    public int b;
}


public static class Program
{
    public static void Main2()
    {
        RuntimeTypeModel.Default.Add(typeof(float3), false).Add("x", "y", "z");
        
        AA aa = new AA() 
        {
            a = 1, 
            pos = new float3(1, 2, 3) 
        };

        using MemoryStream stream = new MemoryStream();
        
        ProtoBuf.Serializer.Serialize(stream, aa);

        byte[] bytes = stream.ToArray();
        
        using MemoryStream stream2 = new MemoryStream(bytes, 0, bytes.Length);
        AA aa1 = ProtoBuf.Serializer.Deserialize(typeof (AA), stream2) as AA;
        Console.WriteLine($"{aa1.a} {aa1.pos}");
    }
}
