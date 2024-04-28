using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET
{
    public class EventSystem : Singleton<EventSystem>, ISingletonUpdate, ISingletonLateUpdate
    {
        // 用于存储单一类型的系统
        private class OneTypeSystems
        {
            // 存储该类型下的所有系统实例，键为系统类型，值为该类型下的所有系统实例
            public readonly UnOrderMultiMap<Type, object> Map = new();
            // 用于标记该类型下的系统是否需要加入到队列中执行，数组长度为InstanceQueueIndex的枚举值数量
            public readonly bool[] QueueFlag = new bool[(int)InstanceQueueIndex.Max];
        }

        // 用于管理所有类型的系统
        private class TypeSystems
        {
            // 存储每种类型对应的系统实例
            private readonly Dictionary<Type, OneTypeSystems> typeSystemsMap = new();

            // 获取或创建某个类型的系统集合
            public OneTypeSystems GetOrCreateOneTypeSystems(Type type)
            {
                OneTypeSystems systems = null;
                // 尝试从字典中获取该类型的系统集合
                this.typeSystemsMap.TryGetValue(type, out systems);
                // 如果已存在则直接返回，否则创建新的实例并添加到字典中
                if (systems != null)
                {
                    return systems;
                }

                systems = new OneTypeSystems();
                this.typeSystemsMap.Add(type, systems);
                return systems;
            }

            // 获取某个类型的系统集合，如果不存在则返回null
            public OneTypeSystems GetOneTypeSystems(Type type)
            {
                OneTypeSystems systems = null;
                this.typeSystemsMap.TryGetValue(type, out systems);
                return systems;
            }

            // 获取某个类型下指定系统类型的所有系统实例，如果不存在则返回null
            public List<object> GetSystems(Type type, Type objectType)
            {
                OneTypeSystems oneTypeSystems = null;
                // 尝试获取该类型下的系统集合
                if (!this.typeSystemsMap.TryGetValue(type, out oneTypeSystems))
                {
                    return null;
                }

                // 尝试从系统集合中获取指定类型的系统实例列表
                if (!oneTypeSystems.Map.TryGetValue(objectType, out List<object> objectList))
                {
                    return null;
                }

                return objectList;
            }
        }
        // 存储事件信息，包含事件实例和场景类型
        private class EventInfo
        {
            public IEvent IEvent { get; } // 事件实例
            public SceneType SceneType { get; } // 场景类型

            public EventInfo(IEvent iEvent, SceneType sceneType)
            {
                this.IEvent = iEvent;
                this.SceneType = sceneType;
            }
        }

        #region 数据源
        // 记录名字跟类型的映射，用于存储各种类型的名称与其对应的类型
        private readonly Dictionary<string, Type> allTypes = new Dictionary<string, Type>();

        // 记录 Attribute 跟 GetType 的映射关系，用于存储各种特性和其应用的类型
        private readonly UnOrderMultiMapSet<Type, Type> attributetypes = new UnOrderMultiMapSet<Type, Type>();

        // 存储发布的事件参数类型以及其对应的订阅者列表
        private readonly Dictionary<Type, List<EventInfo>> allEvents = new Dictionary<Type, List<EventInfo>>();

        // 存储调用的参数类型以及其对应的被调用者列表
        private Dictionary<Type, Dictionary<int, object>> allInvokes = new Dictionary<Type, Dictionary<int, object>>();

        // 存储不同类型系统的容器，用于存储每种类型系统的信息
        private TypeSystems typeSystems = new TypeSystems();

        //创建了3个队列Queue<long>
        private readonly Queue<long>[] queues = new Queue<long>[(int)InstanceQueueIndex.Max];
        #endregion
        public EventSystem()
        {
            //实例化队列
            for (int i = 0; i < this.queues.Length; i++)
            {
                this.queues[i] = new Queue<long>();
            }
        }

        //加数据
        public void Add(Dictionary<string, Type> strTypeDic)
        {
            //将类型的全名作为键，类型对象作为值

            // 清空已有类型映射和属性类型映射
            this.allTypes.Clear();
            this.attributetypes.Clear();

            // 加入数据
            foreach ((string fullName, Type type) in strTypeDic)
            {
                // 添加类型到全局类型映射
                this.allTypes[fullName] = type;

                // 如果类型是抽象的，则跳过
                if (type.IsAbstract)
                {
                    continue;
                }

                // 获取类型上所有 BaseAttribute 的实例
                object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), true);

                // 遍历 BaseAttribute
                foreach (object obj in objects)
                {
                    // 记录 BaseAttribute 类型和对应的类型
                    this.attributetypes.Add(obj.GetType(), type);
                }
            }

            // 创建新的类型系统
            this.typeSystems = new TypeSystems();

            // 从attributetypes获取所有带有 ObjectSystemAttribute 的类型给typeSystems赋值
            foreach (Type objType in this.GetTypes(typeof(ObjectSystemAttribute)))
            {
                // 创建类型实例
                object obj = Activator.CreateInstance(objType);

                // 如果类型实现了 ISystemType 接口
                if (obj is ISystemType iSystemType)
                {
                    // 获取或创建 OneTypeSystems
                    OneTypeSystems oneTypeSystems = this.typeSystems.GetOrCreateOneTypeSystems(iSystemType.GetType());

                    // 将系统类型和实例添加到映射
                    oneTypeSystems.Map.Add(iSystemType.GetSystemType(), obj);

                    // 获取实例队列索引
                    InstanceQueueIndex index = iSystemType.GetInstanceQueueIndex();

                    // 如果索引有效，设置对应标记为 true
                    if (index > InstanceQueueIndex.None && index < InstanceQueueIndex.Max)
                    {
                        oneTypeSystems.QueueFlag[(int)index] = true;
                    }
                }
            }

            // 清空所有事件字典
            this.allEvents.Clear();

            // 遍历所有 EventAttribute 类型的类型
            foreach (Type type in attributetypes[typeof(EventAttribute)])
            {
                // 创建事件实例
                IEvent ieventObj = Activator.CreateInstance(type) as IEvent;

                // 如果实例为空，抛出异常
                if (ieventObj == null)
                {
                    throw new Exception($"objType not is AEvent: {type.Name}");
                }

                // 获取类型上的 EventAttribute 实例数组
                object[] eventAttrs = type.GetCustomAttributes(typeof(EventAttribute), false);

                // 遍历 EventAttribute 实例数组
                foreach (object eventAttr in eventAttrs)
                {
                    // 强制转换为 EventAttribute
                    EventAttribute eventAttribute = eventAttr as EventAttribute;

                    // 获取事件类型
                    Type eventType = ieventObj.getType;

                    // 创建事件信息
                    EventInfo eventInfo = new EventInfo(ieventObj, eventAttribute.SceneType);

                    // 如果事件类型不存在于 allEvents 字典中，则创建新的列表
                    if (!this.allEvents.ContainsKey(eventType))
                    {
                        this.allEvents.Add(eventType, new List<EventInfo>());
                    }

                    // 将事件信息添加到对应事件类型的列表中
                    this.allEvents[eventType].Add(eventInfo);
                }
            }

            // 创建新的 Invoke 映射字典
            this.allInvokes = new Dictionary<Type, Dictionary<int, object>>();

            // 遍历所有 InvokeAttribute 类型的类型
            foreach (Type type in attributetypes[typeof(InvokeAttribute)])
            {
                // 创建类型实例
                object obj = Activator.CreateInstance(type);
                IInvoke iInvoke = obj as IInvoke;

                // 如果实例为空，抛出异常
                if (iInvoke == null)
                {
                    throw new Exception($"objType not is callback: {type.Name}");
                }

                // 获取类型上的 InvokeAttribute 实例数组
                object[] attrs = type.GetCustomAttributes(typeof(InvokeAttribute), false);

                // 遍历 InvokeAttribute 实例数组
                foreach (object attr in attrs)
                {
                    // 获取或创建类型实例的 Invoke 映射字典
                    if (!this.allInvokes.TryGetValue(iInvoke.getType, out var dict))
                    {
                        dict = new Dictionary<int, object>();
                        this.allInvokes.Add(iInvoke.getType, dict);
                    }

                    // 强制转换为 InvokeAttribute
                    InvokeAttribute invokeAttribute = attr as InvokeAttribute;

                    try
                    {
                        // 添加回调对象到字典中
                        dict.Add(invokeAttribute.Type, obj);
                    }
                    catch (Exception e)
                    {
                        // 如果添加重复，则抛出异常
                        throw new Exception($"action objType duplicate: {iInvoke.getType.Name} {invokeAttribute.Type}", e);
                    }
                }
            }
        }

        #region  一些get方法
        //根据给定的系统属性类型获取对应类型的集合
        public HashSet<Type> GetTypes(Type systemAttributeType)
        {
            // 检查 attributetypes 是否包含指定的系统属性类型
            if (!this.attributetypes.ContainsKey(systemAttributeType))
            {
                // 如果不包含，返回一个空的 HashSet
                return new HashSet<Type>();
            }

            // 如果包含，返回对应系统属性类型的 HashSet
            return this.attributetypes[systemAttributeType];
        }
        public Dictionary<string, Type> GetAllTypes()
        {
            return allTypes;
        }
        public Type GetType(string typeName)
        {
            return this.allTypes[typeName];
        }
        #endregion


        #region  一些system相关
        public void RegisterSystem(Entity component)
        {
            // 获取实体组件的类型
            Type type = component.GetType();

            // 获取与该类型相关的系统信息
            OneTypeSystems oneTypeSystems = this.typeSystems.GetOneTypeSystems(type);

            // 如果没有找到与该类型相关的系统信息，则直接中断
            if (oneTypeSystems == null)
            {
                return;
            }

            // 遍历每种实例队列的标志
            for (int i = 0; i < oneTypeSystems.QueueFlag.Length; ++i)
            {
                // 如果当前实例队列标志为false，则跳过
                if (!oneTypeSystems.QueueFlag[i])
                {
                    continue;
                }

                // 将当前实例的InstanceId添加到相应的实例队列中
                this.queues[i].Enqueue(component.InstanceId);
            }
        }

        public void DeserializeSystem(Entity component)
        {
            // 获取与组件类型相关的所有 IDeserializeSystem 系统
            List<object> iDeserializeSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IDeserializeSystem));

            // 如果没有找到与该组件类型相关的 IDeserializeSystem 系统，则直接返回
            if (iDeserializeSystems == null)
            {
                return;
            }

            // 遍历所有的 IDeserializeSystem 系统
            foreach (IDeserializeSystem deserializeSystem in iDeserializeSystems)
            {
                // 如果当前系统为空，则跳过
                if (deserializeSystem == null)
                {
                    continue;
                }

                try
                {
                    // 运行当前的 IDeserializeSystem 系统来反序列化组件
                    deserializeSystem.Run(component);
                }
                catch (Exception e)
                {
                    // 捕获异常并记录日志
                    Log.Error(e);
                }
            }
        }


        // 获得组件相关
        public void GetComponentSystem(Entity entity, Entity component)
        {
            // 获取与实体类型相关的所有 IGetComponentSystem 系统
            List<object> iGetSystem = this.typeSystems.GetSystems(entity.GetType(), typeof(IGetComponentSystem));

            // 如果没有找到与该实体类型相关的 IGetComponentSystem 系统，则直接返回
            if (iGetSystem == null)
            {
                return;
            }

            // 遍历所有的 IGetComponentSystem 系统
            foreach (IGetComponentSystem getSystem in iGetSystem)
            {
                // 如果当前系统为空，则跳过
                if (getSystem == null)
                {
                    continue;
                }

                try
                {
                    // 运行当前的 IGetComponentSystem 系统来获取组件
                    getSystem.Run(entity, component);
                }
                catch (Exception e)
                {
                    // 捕获异常并记录日志
                    Log.Error(e);
                }
            }
        }


        // 添加组件系统
        public void AddComponentSystem(Entity entity, Entity component)
        {
            // 获取与实体类型相关的所有 IAddComponentSystem 系统
            List<object> iAddSystem = this.typeSystems.GetSystems(entity.GetType(), typeof(IAddComponentSystem));

            // 如果没有找到与该实体类型相关的 IAddComponentSystem 系统，则直接返回
            if (iAddSystem == null)
            {
                return;
            }

            // 遍历所有的 IAddComponentSystem 系统
            foreach (IAddComponentSystem addComponentSystem in iAddSystem)
            {
                // 如果当前系统为空，则跳过
                if (addComponentSystem == null)
                {
                    continue;
                }

                try
                {
                    // 运行当前的 IAddComponentSystem 系统来添加组件
                    addComponentSystem.Run(entity, component);
                }
                catch (Exception e)
                {
                    // 捕获异常并记录日志
                    Log.Error(e);
                }
            }
        }
        #endregion


        #region Awake相关
        public void Awake(Entity component)
        {
            List<object> iAwakeSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IAwakeSystem));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<P1>(Entity component, P1 p1)
        {
            List<object> iAwakeSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IAwakeSystem<P1>));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem<P1> aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component, p1);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<P1, P2>(Entity component, P1 p1, P2 p2)
        {
            List<object> iAwakeSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IAwakeSystem<P1, P2>));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem<P1, P2> aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component, p1, p2);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<P1, P2, P3>(Entity component, P1 p1, P2 p2, P3 p3)
        {
            List<object> iAwakeSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IAwakeSystem<P1, P2, P3>));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem<P1, P2, P3> aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component, p1, p2, p3);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<P1, P2, P3, P4>(Entity component, P1 p1, P2 p2, P3 p3, P4 p4)
        {
            List<object> iAwakeSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IAwakeSystem<P1, P2, P3, P4>));
            if (iAwakeSystems == null)
            {
                return;
            }

            foreach (IAwakeSystem<P1, P2, P3, P4> aAwakeSystem in iAwakeSystems)
            {
                if (aAwakeSystem == null)
                {
                    continue;
                }

                try
                {
                    aAwakeSystem.Run(component, p1, p2, p3, p4);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        #endregion

        // 加载方法，用于处理加载队列中的实体组件
        public void Load()
        {
            // 获取加载队列
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.Load];
            // 获取队列中实体数量
            int count = queue.Count;
            // 循环处理队列中的每个实体
            while (count-- > 0)
            {
                // 从队列中取出一个实体的实例ID
                long instanceId = queue.Dequeue();
                // 根据实例ID获取实体组件
                Entity component = Root.Instance.Get(instanceId);
                // 如果实体组件为空，则继续处理下一个实体
                if (component == null)
                {
                    continue;
                }

                // 如果实体已经被释放，则继续处理下一个实体
                if (component.IsDisposed)
                {
                    continue;
                }

                // 获取该实体对应的所有加载系统
                List<object> iLoadSystems = this.typeSystems.GetSystems(component.GetType(), typeof(ILoadSystem));
                // 如果没有加载系统与该实体关联，则继续处理下一个实体
                if (iLoadSystems == null)
                {
                    continue;
                }

                // 将实体的实例ID重新加入加载队列，保持队列中实体不变
                queue.Enqueue(instanceId);

                // 遍历所有加载系统
                foreach (ILoadSystem iLoadSystem in iLoadSystems)
                {
                    try
                    {
                        // 运行加载系统的方法，处理当前实体
                        iLoadSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        // 捕获并记录异常信息
                        Log.Error(e);
                    }
                }
            }
        }


        // 销毁方法，用于处理销毁实体组件时调用的系统
        public void Destroy(Entity component)
        {
            // 获取实体组件对应的所有销毁系统
            List<object> iDestroySystems = this.typeSystems.GetSystems(component.GetType(), typeof(IDestroySystem));
            // 如果没有任何销毁系统与该实体关联，则直接返回
            if (iDestroySystems == null)
            {
                return;
            }

            // 遍历所有销毁系统
            foreach (IDestroySystem iDestroySystem in iDestroySystems)
            {
                // 如果当前销毁系统为空，则继续处理下一个
                if (iDestroySystem == null)
                {
                    continue;
                }

                try
                {
                    // 运行当前销毁系统的方法，处理该实体组件的销毁逻辑
                    iDestroySystem.Run(component);
                }
                catch (Exception e)
                {
                    // 捕获并记录任何可能的异常信息
                    Log.Error(e);
                }
            }
        }


        // Update 方法，用于处理更新实体组件时调用的系统
        public void Update()
        {
            // 获取 Update 实例队列
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.Update];
            // 获取队列中的实例数量
            int count = queue.Count;
            // 遍历队列中的每一个实例
            while (count-- > 0)
            {
                // 出队一个实例的 ID
                long instanceId = queue.Dequeue();
                // 根据实例 ID 获取实体组件
                Entity component = Root.Instance.Get(instanceId);
                // 如果实体组件为空，说明已经被销毁，跳过本次循环
                if (component == null)
                {
                    continue;
                }

                // 如果实体组件已经被标记为已释放，跳过本次循环
                if (component.IsDisposed)
                {
                    continue;
                }

                // 获取该实体组件对应的所有更新系统
                List<object> iUpdateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(IUpdateSystem));
                // 如果没有任何更新系统与该实体关联，则跳过本次循环
                if (iUpdateSystems == null)
                {
                    continue;
                }

                // 将实例重新入队，以确保下一次更新
                queue.Enqueue(instanceId);

                // 遍历所有更新系统
                foreach (IUpdateSystem iUpdateSystem in iUpdateSystems)
                {
                    try
                    {
                        // 运行当前更新系统的方法，处理该实体组件的更新逻辑
                        iUpdateSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        // 捕获并记录任何可能的异常信息
                        Log.Error(e);
                    }
                }
            }
        }


        // LateUpdate 方法，用于处理后期更新实体组件时调用的系统
        public void LateUpdate()
        {
            // 获取 LateUpdate 实例队列
            Queue<long> queue = this.queues[(int)InstanceQueueIndex.LateUpdate];
            // 获取队列中的实例数量
            int count = queue.Count;
            // 遍历队列中的每一个实例
            while (count-- > 0)
            {
                // 出队一个实例的 ID
                long instanceId = queue.Dequeue();
                // 根据实例 ID 获取实体组件
                Entity component = Root.Instance.Get(instanceId);
                // 如果实体组件为空，说明已经被销毁，跳过本次循环
                if (component == null)
                {
                    continue;
                }

                // 如果实体组件已经被标记为已释放，跳过本次循环
                if (component.IsDisposed)
                {
                    continue;
                }

                // 获取该实体组件对应的所有后期更新系统
                List<object> iLateUpdateSystems = this.typeSystems.GetSystems(component.GetType(), typeof(ILateUpdateSystem));
                // 如果没有任何后期更新系统与该实体关联，则跳过本次循环
                if (iLateUpdateSystems == null)
                {
                    continue;
                }

                // 将实例重新入队，以确保下一次后期更新
                queue.Enqueue(instanceId);

                // 遍历所有后期更新系统
                foreach (ILateUpdateSystem iLateUpdateSystem in iLateUpdateSystems)
                {
                    try
                    {
                        // 运行当前后期更新系统的方法，处理该实体组件的后期更新逻辑
                        iLateUpdateSystem.Run(component);
                    }
                    catch (Exception e)
                    {
                        // 捕获并记录任何可能的异常信息
                        Log.Error(e);
                    }
                }
            }
        }

        #region Publish相关
        // 异步发布事件的方法，用于在场景中异步发布特定类型的事件
        // T: 事件的类型，必须是结构体
        // scene: 要发布事件的场景
        // a: 事件的参数
        // 返回一个异步任务对象，表示事件发布的结果
        public async ETTask PublishAsync<T>(Scene scene, T a) where T : struct
        {
            // 获取事件类型为 T 的所有事件信息列表
            List<EventInfo> iEvents;
            if (!this.allEvents.TryGetValue(typeof(T), out iEvents))
            {
                return; // 如果没有找到与事件类型 T 相关的事件信息列表，则直接返回
            }
            // 使用 using 语句创建一个临时的 ListComponent 对象，用于存储异步任务对象
            using ListComponent<ETTask> list = ListComponent<ETTask>.Create();

            // 遍历事件信息列表，处理每个事件
            foreach (EventInfo eventInfo in iEvents)
            {
                // 如果事件的场景类型不匹配当前场景类型，并且事件的场景类型不是 None，则跳过本次循环
                if (scene.SceneType != eventInfo.SceneType && eventInfo.SceneType != SceneType.None)
                {
                    continue;
                }

                // 尝试将事件转换为 AEvent<T> 类型的事件对象，如果转换失败则记录错误并跳过本次循环
                if (!(eventInfo.IEvent is AEvent<T> aEvent))
                {
                    Log.Error($"event error: {eventInfo.IEvent.GetType().Name}");
                    continue;
                }

                // 将事件处理方法的返回值（异步任务对象）添加到列表中
                list.Add(aEvent.Handle(scene, a));
            }

            try
            {
                // 等待所有异步任务对象完成
                await ETTaskHelper.WaitAll(list);
            }
            catch (Exception e)
            {
                // 捕获并记录任何可能的异常信息
                Log.Error(e);
            }

        }

        public void Publish<T>(Scene scene, T a) where T : struct
        {
            List<EventInfo> iEvents;
            if (!this.allEvents.TryGetValue(typeof(T), out iEvents))
            {
                return;
            }

            SceneType sceneType = scene.SceneType;
            foreach (EventInfo eventInfo in iEvents)
            {
                if (sceneType != eventInfo.SceneType && eventInfo.SceneType != SceneType.None)
                {
                    continue;
                }


                if (!(eventInfo.IEvent is AEvent<T> aEvent))
                {
                    Log.Error($"event error: {eventInfo.IEvent.GetType().Name}");
                    continue;
                }

                aEvent.Handle(scene, a).Coroutine();
            }
        }
        #endregion

        #region Invoke相关
        // Invoke跟Publish的区别(特别注意)
        // Invoke类似函数，必须有被调用方，否则异常，调用者跟被调用者属于同一模块，比如MoveComponent中的Timer计时器，调用跟被调用的代码均属于移动模块
        // 既然Invoke跟函数一样，那么为什么不使用函数呢? 因为有时候不方便直接调用，比如Config加载，在客户端跟服务端加载方式不一样。比如TimerComponent需要根据Id分发
        // 注意，不要把Invoke当函数使用，这样会造成代码可读性降低，能用函数不要用Invoke
        // publish是事件，抛出去可以没人订阅，调用者跟被调用者属于两个模块，比如任务系统需要知道道具使用的信息，则订阅道具使用事件
        public void Invoke<A>(int type, A args) where A : struct
        {
            if (!this.allInvokes.TryGetValue(typeof(A), out var invokeHandlers))
            {
                throw new Exception($"Invoke error: {typeof(A).Name}");
            }
            if (!invokeHandlers.TryGetValue(type, out var invokeHandler))
            {
                throw new Exception($"Invoke error: {typeof(A).Name} {type}");
            }

            var aInvokeHandler = invokeHandler as AInvokeHandler<A>;
            if (aInvokeHandler == null)
            {
                throw new Exception($"Invoke error, not AInvokeHandler: {typeof(A).Name} {type}");
            }

            aInvokeHandler.Handle(args);
        }

        public T Invoke<A, T>(int type, A args) where A : struct
        {
            if (!this.allInvokes.TryGetValue(typeof(A), out var invokeHandlers))
            {
                throw new Exception($"Invoke error: {typeof(A).Name}");
            }
            if (!invokeHandlers.TryGetValue(type, out var invokeHandler))
            {
                throw new Exception($"Invoke error: {typeof(A).Name} {type}");
            }

            var aInvokeHandler = invokeHandler as AInvokeHandler<A, T>;
            if (aInvokeHandler == null)
            {
                throw new Exception($"Invoke error, not AInvokeHandler: {typeof(T).Name} {type}");
            }

            return aInvokeHandler.Handle(args);
        }

        public void Invoke<A>(A args) where A : struct
        {
            Invoke(0, args);
        }

        public T Invoke<A, T>(A args) where A : struct
        {
            return Invoke<A, T>(0, args);
        }
        #endregion
    }
}
