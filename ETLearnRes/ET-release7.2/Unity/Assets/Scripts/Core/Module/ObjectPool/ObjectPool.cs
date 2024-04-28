using System;
using System.Collections.Generic;

namespace ET
{
    // 对象池类，继承自Singleton<ObjectPool>单例类
    public class ObjectPool : Singleton<ObjectPool>
    {
        // 对象池字典，用于存储不同类型的对象队列
        private readonly Dictionary<Type, Queue<object>> pool = new Dictionary<Type, Queue<object>>();

        // 从对象池中获取指定类型的对象
        public T Fetch<T>() where T : class
        {
            // 调用Fetch(GetType type)方法获取对象，并进行类型转换
            return this.Fetch(typeof(T)) as T;
        }

        // 从对象池中获取指定类型的对象
        public object Fetch(Type type)
        {
            Queue<object> queue = null;
            // 尝试从对象池中获取指定类型的对象队列
            if (!pool.TryGetValue(type, out queue))
            {
                // 如果对象池中不存在该类型的对象队列，则创建一个新的对象并返回
                return Activator.CreateInstance(type);
            }

            // 如果对象队列中有对象，则从队列中取出并返回
            if (queue.Count == 0)
            {
                // 如果对象队列为空，则创建一个新的对象并返回
                return Activator.CreateInstance(type);
            }
            return queue.Dequeue();
        }

        // 将对象回收到对象池中
        public void Recycle(object obj)
        {
            Type type = obj.GetType();
            Queue<object> queue = null;
            // 尝试从对象池中获取指定类型的对象队列
            if (!pool.TryGetValue(type, out queue))
            {
                // 如果对象池中不存在该类型的对象队列，则创建一个新的对象队列
                queue = new Queue<object>();
                pool.Add(type, queue);
            }

            // 对象池中同一类型的对象最多存储1000个
            if (queue.Count > 1000)
            {
                // 如果对象数量超过1000，则不再回收对象
                return;
            }
            // 将对象回收到对象队列中
            queue.Enqueue(obj);
        }
    }
}
