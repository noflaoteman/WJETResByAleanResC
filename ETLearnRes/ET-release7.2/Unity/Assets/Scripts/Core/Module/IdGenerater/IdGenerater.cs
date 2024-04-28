using System;
using System.Runtime.InteropServices;

namespace ET
{
    // IdStruct 结构体，用于表示一个32位整数的不同部分
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IdStruct
    {
        public uint Time;    // 时间部分，30位
        public int Process;  // 进程部分，18位
        public ushort Value; // 值部分，16位

        // 将IdStruct转换为long类型
        public long ToLong()
        {
            ulong result = 0;
            result |= this.Value;
            result |= (ulong)this.Process << 16;
            result |= (ulong)this.Time << 34;
            return (long)result;
        }

        // 构造函数，根据给定的时间、进程和值来初始化IdStruct
        public IdStruct(uint time, int process, ushort value)
        {
            this.Process = process;
            this.Time = time;
            this.Value = value;
        }

        // 构造函数，根据给定的long类型id来初始化IdStruct
        public IdStruct(long id)
        {
            ulong result = (ulong)id;
            this.Value = (ushort)(result & ushort.MaxValue);
            result >>= 16;
            this.Process = (int)(result & IdGenerater.Mask18bit);
            result >>= 18;
            this.Time = (uint)result;
        }

        // 返回IdStruct的字符串表示
        public override string ToString()
        {
            return $"process: {this.Process}, time: {this.Time}, value: {this.Value}";
        }
    }

    // InstanceIdStruct 结构体，用于表示一个32位整数的不同部分（用于实例Id）
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct InstanceIdStruct
    {
        public uint Time;   // 时间部分，28位
        public int Process; // 进程部分，18位
        public uint Value;  // 值部分，18位

        // 将InstanceIdStruct转换为long类型
        public long ToLong()
        {
            ulong result = 0;
            result |= this.Value;
            result |= (ulong)this.Process << 18;
            result |= (ulong)this.Time << 36;
            return (long)result;
        }

        // 构造函数，根据给定的时间、进程和值来初始化InstanceIdStruct
        public InstanceIdStruct(uint time, int process, uint value)
        {
            this.Time = time;
            this.Process = process;
            this.Value = value;
        }

        // 构造函数，根据给定的long类型id来初始化InstanceIdStruct
        public InstanceIdStruct(long id)
        {
            ulong result = (ulong)id;
            this.Value = (uint)(result & IdGenerater.Mask18bit);
            result >>= 18;
            this.Process = (int)(result & IdGenerater.Mask18bit);
            result >>= 18;
            this.Time = (uint)result;
        }

        // 构造函数，用于给SceneId使用
        public InstanceIdStruct(int process, uint value)
        {
            this.Time = 0;
            this.Process = process;
            this.Value = value;
        }

        // 返回InstanceIdStruct的字符串表示
        public override string ToString()
        {
            return $"process: {this.Process}, value: {this.Value} time: {this.Time}";
        }
    }

    // UnitIdStruct 结构体，用于表示一个64位整数的不同部分（用于单位Id）
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UnitIdStruct
    {
        public uint Time;        // 时间部分，30位（34年）
        public ushort Zone;      // 区域部分，10位（1024个区）
        public byte ProcessMode; // 进程模式部分，8位（Process % 256，每个区最多256个进程）
        public ushort Value;     // 值部分，16位（每秒每个进程最大16K个Unit）

        // 将UnitIdStruct转换为long类型
        public long ToLong()
        {
            ulong result = 0;
            result |= this.Value;
            result |= (uint)this.ProcessMode << 16;
            result |= (ulong)this.Zone << 24;
            result |= (ulong)this.Time << 34;
            return (long)result;
        }

        // 构造函数，根据给定的区域、进程、时间和值来初始化UnitIdStruct
        public UnitIdStruct(int zone, int process, uint time, ushort value)
        {
            this.Time = time;
            this.ProcessMode = (byte)(process % 256);
            this.Value = value;
            this.Zone = (ushort)zone;
        }

        // 构造函数，根据给定的long类型id来初始化UnitIdStruct
        public UnitIdStruct(long id)
        {
            ulong result = (ulong)id;
            this.Value = (ushort)(result & ushort.MaxValue);
            result >>= 16;
            this.ProcessMode = (byte)(result & byte.MaxValue);
            result >>= 8;
            this.Zone = (ushort)(result & 0x03ff);
            result >>= 10;
            this.Time = (uint)result;
        }

        // 返回UnitIdStruct的字符串表示
        public override string ToString()
        {
            return $"ProcessMode: {this.ProcessMode}, value: {this.Value} time: {this.Time}";
        }

        // 根据单位Id获取区域
        public static int GetUnitZone(long unitId)
        {
            int v = (int)((unitId >> 24) & 0x03ff); // 取出10bit
            return v;
        }
    }

    // IdGenerater 类，用于生成不同类型的Id
    public class IdGenerater : Singleton<IdGenerater>
    {
        public const int Mask18bit = 0x03ffff; // 18位掩码

        public const int MaxZone = 1024; // 最大区域数

        private long epoch2020; // 2020年1月1日的时间戳
        private ushort value; // Id值
        private uint lastIdTime; // 上一个Id的时间戳

        private long epochThisYear; // 今年的开始时间戳
        private uint instanceIdValue; // 实例Id值
        private uint lastInstanceIdTime; // 上一个实例Id的时间戳

        private ushort unitIdValue; // 单位Id值
        private uint lastUnitIdTime; // 上一个单位Id的时间戳

        // 构造函数，初始化各种时间戳和值
        public IdGenerater()
        {
            long epoch1970tick = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
            this.epoch2020 = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000 - epoch1970tick;
            this.epochThisYear = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000 - epoch1970tick;

            this.lastInstanceIdTime = TimeSinceThisYear();
            if (this.lastInstanceIdTime <= 0)
            {
                Log.Warning($"lastInstanceIdTime less than 0: {this.lastInstanceIdTime}");
                this.lastInstanceIdTime = 1;
            }
            this.lastIdTime = TimeSince2020();
            if (this.lastIdTime <= 0)
            {
                Log.Warning($"lastIdTime less than 0: {this.lastIdTime}");
                this.lastIdTime = 1;
            }
            this.lastUnitIdTime = TimeSince2020();
            if (this.lastUnitIdTime <= 0)
            {
                Log.Warning($"lastUnitIdTime less than 0: {this.lastUnitIdTime}");
                this.lastUnitIdTime = 1;
            }
        }

        // 获取自2020年以来的时间戳
        private uint TimeSince2020()
        {
            uint a = (uint)((TimeInfo.Instance.FrameTime - this.epoch2020) / 1000);
            return a;
        }

        // 获取自今年以来的时间戳
        private uint TimeSinceThisYear()
        {
            uint a = (uint)((TimeInfo.Instance.FrameTime - this.epochThisYear) / 1000);
            return a;
        }

        // 生成实例Id
        public long GenerateInstanceId()
        {
            uint time = TimeSinceThisYear();

            if (time > this.lastInstanceIdTime)
            {
                this.lastInstanceIdTime = time;
                this.instanceIdValue = 0;
            }
            else
            {
                ++this.instanceIdValue;

                if (this.instanceIdValue > IdGenerater.Mask18bit - 1) // 18位
                {
                    ++this.lastInstanceIdTime; // 借用下一秒
                    this.instanceIdValue = 0;

                    Log.Error($"instanceid count per sec overflow: {time} {this.lastInstanceIdTime}");
                }
            }

            InstanceIdStruct instanceIdStruct = new InstanceIdStruct(this.lastInstanceIdTime, Options.Instance.Process, this.instanceIdValue);
            return instanceIdStruct.ToLong();
        }

        // 生成Id
        public long GenerateId()
        {
            uint time = TimeSince2020();

            if (time > this.lastIdTime)
            {
                this.lastIdTime = time;
                this.value = 0;
            }
            else
            {
                ++this.value;

                if (value > ushort.MaxValue - 1)
                {
                    this.value = 0;
                    ++this.lastIdTime; // 借用下一秒
                    Log.Error($"id count per sec overflow: {time} {this.lastIdTime}");
                }
            }

            IdStruct idStruct = new IdStruct(this.lastIdTime, Options.Instance.Process, value);
            return idStruct.ToLong();
        }

        // 生成单位Id
        public long GenerateUnitId(int zone)
        {
            if (zone > MaxZone)
            {
                throw new Exception($"zone > MaxZone: {zone}");
            }
            uint time = TimeSince2020();

            if (time > this.lastUnitIdTime)
            {
                this.lastUnitIdTime = time;
                this.unitIdValue = 0;
            }
            else
            {
                ++this.unitIdValue;

                if (this.unitIdValue > ushort.MaxValue - 1)
                {
                    this.unitIdValue = 0;
                    ++this.lastUnitIdTime; // 借用下一秒
                    Log.Error($"unitid count per sec overflow: {time} {this.lastUnitIdTime}");
                }
            }

            UnitIdStruct unitIdStruct = new UnitIdStruct(zone, Options.Instance.Process, this.lastUnitIdTime, this.unitIdValue);
            return unitIdStruct.ToLong();
        }
    }
}
