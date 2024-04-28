using System;

namespace ET
{
    // 定义事件接口
    public interface IEvent
    {
        Type getType { get; } // 获取事件的类型
    }

    // 定义抽象事件基类，泛型参数A表示事件的数据类型
    public abstract class AEvent<T> : IEvent where T : struct
    {
        public Type getType
        {
            get
            {
                return typeof(T);
            }
        }

        protected abstract ETTask Run(Scene scene, T a);

        public async ETTask Handle(Scene scene, T a)
        {
            try
            {
                // 调用子类实现的Run方法处理事件,其实这就是一个多态的行为
                await Run(scene, a);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
