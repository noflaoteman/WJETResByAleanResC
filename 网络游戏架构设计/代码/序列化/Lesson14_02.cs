using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ET;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using ProtoBuf;
using ProtoBuf.Meta;
using Unity.Mathematics;

namespace Lesson14_02;

public class AA
{
    [BsonId]
    public long Id;
    
    [BsonElement]
    [BsonDefaultValue(0)]
    [BsonIgnoreIfDefault]
    public int a;
    
    [BsonIgnore]
    public int b;

    [BsonElement("p")]
    private float3 pos = new float3(1, 2, 3);
}

//[BsonIgnoreExtraElements]
public class BB: AA
{
    public int c;
}

public class BB2: AA
{
    public int c;
    public int d;
}

public class BB3: AA
{
    public Dictionary<string, long> e = new() {{"0", 1}, {"1", 2}};
}

public class BB4: AA
{
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
    public Dictionary<int, long> e = new() {{0, 1}, {1, 2}};
}


public static class Program
{
    public static void Main()
    {
        Entry.Init();
        Init.Start();

        
        //Test1();
        //Test2();
        //Test3();
        //Test4();
        //Test5();
        //Test6();
        Test7();

            
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


    public static void Test1()
    {
        AA aa = new AA() 
        {
            a = 1, 
            b = 2,
        };

        byte[] bytes = MongoHelper.Serialize(aa);
        
        AA aa1 = MongoHelper.Deserialize(typeof(AA), bytes) as AA;
        
        Console.WriteLine(MongoHelper.ToJson(aa1));
    }
    
    public static void Test2()
    {
        BB bb = new BB() 
        {
            a = 1, 
            b = 2,
            c = 3,
        };
        
        Console.WriteLine(MongoHelper.ToJson(bb));
    }
    
    public static void Test3()
    {
        BB2 bb = new BB2() 
        {
            a = 1, 
            b = 2,
            c = 3,
            d = 4,
        };
        byte[] bytes = MongoHelper.Serialize(bb);
        BB bbb = MongoHelper.Deserialize(typeof(BB), bytes) as BB;
        Console.WriteLine(MongoHelper.ToJson(bbb));
    }
    
    public static void Test4()
    {
        BB3 bb = new BB3();
        Console.WriteLine(MongoHelper.ToJson(bb));
    }
    
    public static void Test5()
    {
        BB4 bb = new BB4();
        Console.WriteLine(MongoHelper.ToJson(bb));
    }
    
    public static void Test6()
    {
        AA bb = new BB2() 
        {
            a = 1, 
            b = 2,
            c = 3,
            d = 4,
        };
        
        string s = MongoHelper.ToJson(bb);
        Console.WriteLine(MongoHelper.ToJson(s));
        BB2 bbb = MongoHelper.FromJson(typeof(AA), s) as BB2;
        Console.WriteLine(MongoHelper.ToJson(bbb));
    }
    
    public static void Test7()
    {
        BsonClassMap.LookupClassMap(typeof(BB2));
        BB2 bbb = MongoHelper.FromJson(typeof(AA), "{ \"_t\" : \"BB2\", \"_id\" : 0, \"a\" : 1, \"pos\" : { \"x\" : 1.0, \"y\" : 2.0, \"z\" : 3.0 }, \"c\" : 3, \"d\" : 4 }") as BB2;
        Console.WriteLine(MongoHelper.ToJson(bbb));
    }
}
