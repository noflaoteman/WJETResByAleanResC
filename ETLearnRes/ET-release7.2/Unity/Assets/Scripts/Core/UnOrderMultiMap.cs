using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary>
    /// 不保持键的顺序的多值字典，用于存储一对多的关系
    /// </summary>
    /// <typeparam name="T">键的类型</typeparam>
    /// <typeparam name="K">值的类型</typeparam>
    public class UnOrderMultiMap<T, K> : Dictionary<T, List<K>>
    {
        /// <summary>
        /// 向字典中添加一个键值对
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        public void Add(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list); // 尝试获取键对应的列表
            if (list == null) // 如果列表不存在
            {
                list = new List<K>(); // 创建新的列表
                base[t] = list; // 将键和列表关联起来
            }
            list.Add(k); // 将值添加到列表中
        }

        /// <summary>
        /// 从字典中移除一个键值对
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        /// <returns>如果成功移除返回true，否则返回false</returns>
        public bool Remove(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list); // 尝试获取键对应的列表
            if (list == null) // 如果列表不存在
            {
                return false; // 返回移除失败
            }
            if (!list.Remove(k)) // 如果无法从列表中移除值
            {
                return false; // 返回移除失败
            }
            if (list.Count == 0) // 如果列表为空
            {
                this.Remove(t); // 从字典中移除该键
            }
            return true; // 返回移除成功
        }

        /// <summary>
        /// 获取指定键对应的所有值的副本
        /// </summary>
        /// <param name="t">键</param>
        /// <returns>该键对应的值数组</returns>
        public K[] GetAll(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list); // 尝试获取键对应的列表
            if (list == null) // 如果列表不存在
            {
                return Array.Empty<K>(); // 返回空数组
            }
            return list.ToArray(); // 返回列表的副本
        }

        /// <summary>
        /// 获取或设置指定键对应的值列表
        /// </summary>
        /// <param name="t">键</param>
        /// <returns>该键对应的值列表</returns>
        public new List<K> this[T t]
        {
            get
            {
                List<K> list;
                this.TryGetValue(t, out list); // 尝试获取键对应的列表
                return list; // 返回列表
            }
        }

        /// <summary>
        /// 获取指定键对应的第一个值
        /// </summary>
        /// <param name="t">键</param>
        /// <returns>指定键对应的第一个值，如果不存在则返回默认值</returns>
        public K GetOne(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list); // 尝试获取键对应的列表
            if (list != null && list.Count > 0) // 如果列表存在且不为空
            {
                return list[0]; // 返回列表的第一个值
            }
            return default(K); // 返回默认值
        }

        /// <summary>
        /// 判断指定键值对是否存在于字典中
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        /// <returns>如果存在返回true，否则返回false</returns>
        public bool Contains(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list); // 尝试获取键对应的列表
            if (list == null) // 如果列表不存在
            {
                return false; // 返回false
            }
            return list.Contains(k); // 判断值是否存在于列表中
        }
    }
}
