using System;

namespace ET
{
    /// <summary>
    /// ����һ���ӿڣ���ʾϵͳ����
    /// </summary>
    public interface ISystemType
    {
        /// <summary>
        /// ��ȡ��ǰʵ��������
        /// </summary>
        /// <returns>���ص�ǰʵ��������</returns>
        Type GetType();

        /// <summary>
        /// ��ȡϵͳ����
        /// </summary>
        /// <returns>����ϵͳ����</returns>
        Type GetSystemType();

        /// <summary>
        /// ��ȡʵ����������
        /// </summary>
        /// <returns>����ʵ����������</returns>
        InstanceQueueIndex GetInstanceQueueIndex();
    }
}
