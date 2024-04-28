using System;

namespace ET
{
	public interface ILateUpdate
	{
	}
	
	public interface ILateUpdateSystem: ISystemType
	{
		void Run(Entity o);
	}

	[ObjectSystem]
	public abstract class LateUpdateSystem<T> : ILateUpdateSystem where T: Entity, ILateUpdate
	{
		void ILateUpdateSystem.Run(Entity o)
		{
			this.LateUpdate((T)o);
		}

		Type ISystemType.GetType()
		{
			return typeof(T);
		}

		Type ISystemType.GetSystemType()
		{
			return typeof(ILateUpdateSystem);
		}

		InstanceQueueIndex ISystemType.GetInstanceQueueIndex()
		{
			return InstanceQueueIndex.LateUpdate;
		}

		protected abstract void LateUpdate(T self);
	}
}
