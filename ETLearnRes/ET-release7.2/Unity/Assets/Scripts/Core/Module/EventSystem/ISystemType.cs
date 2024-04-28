using System;

namespace ET
{
    /// <summary>
    /// 定义一个接口，表示系统类型
    /// </summary>
    public interface ISystemType
    {
        /// <summary>
        /// 获取当前实例的类型
        /// </summary>
        /// <returns>返回当前实例的类型</returns>
        Type GetType();

        /// <summary>
        /// 获取系统类型
        /// </summary>
        /// <returns>返回系统类型</returns>
        Type GetSystemType();

        /// <summary>
        /// 获取实例队列索引
        /// </summary>
        /// <returns>返回实例队列索引</returns>
        InstanceQueueIndex GetInstanceQueueIndex();
    }
}
