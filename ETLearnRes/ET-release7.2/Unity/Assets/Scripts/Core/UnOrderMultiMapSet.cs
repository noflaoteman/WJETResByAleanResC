/**
 * 多重映射结构
 * UnOrderMultiMapSet 类用于表示一种多重映射结构，其中一个键可以对应多个值的集合，且键值对的顺序不固定。
 */
using System.Collections.Generic;

namespace ET
{
    // UnOrderMultiMapSet 类继承自 Dictionary 类，用于实现多重映射结构
    public class UnOrderMultiMapSet<T, K> : Dictionary<T, HashSet<K>>
    {
        // 重用 HashSet，通过索引器实现
        public new HashSet<K> this[T t]
        {
            get
            {
                HashSet<K> set;
                // 尝试获取对应键的 HashSet，若不存在则创建新的 HashSet
                if (!this.TryGetValue(t, out set))
                {
                    set = new HashSet<K>();
                }
                return set;
            }
        }

        // 获取整个字典，用于返回所有键值对
        public Dictionary<T, HashSet<K>> GetDictionary()
        {
            return this;
        }

        // 向映射中添加键值对
        public void Add(T t, K k)
        {
            HashSet<K> hashSet;
            // 尝试获取对应键的 HashSet，若不存在则创建新的 HashSet
            this.TryGetValue(t, out hashSet);
            if (hashSet == null)
            {
                hashSet = new HashSet<K>();
                base[t] = hashSet;
            }
            // 将值添加到 HashSet 中
            hashSet.Add(k);
        }

        // 从映射中移除键值对
        public bool Remove(T t, K k)
        {
            HashSet<K> set;
            // 尝试获取对应键的 HashSet，若不存在则返回 false
            this.TryGetValue(t, out set);
            if (set == null)
            {
                return false;
            }
            // 从 HashSet 中移除值，若不存在则返回 false
            if (!set.Remove(k))
            {
                return false;
            }
            // 若 HashSet 为空，则移除对应键
            if (set.Count == 0)
            {
                this.Remove(t);
            }
            return true;
        }

        // 判断映射中是否包含指定的键值对
        public bool Contains(T t, K k)
        {
            HashSet<K> set;
            // 尝试获取对应键的 HashSet，若不存在则返回 false
            this.TryGetValue(t, out set);
            if (set == null)
            {
                return false;
            }
            // 判断 HashSet 是否包含指定的值
            return set.Contains(k);
        }

        // 获取映射中键值对的总数，重写 Count 属性
        public new int Count
        {
            get
            {
                int count = 0;
                // 遍历所有键值对，累加值的数量
                foreach (KeyValuePair<T, HashSet<K>> kv in this)
                {
                    count += kv.Value.Count;
                }
                return count;
            }
        }
    }
}
