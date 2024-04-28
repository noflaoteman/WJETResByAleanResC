namespace ET
{
    // 定义了实例队列的索引枚举
    public enum InstanceQueueIndex
    {
        // 表示无效的队列索引
        None = -1,
        // 表示更新队列
        Update,
        // 表示晚期更新队列
        LateUpdate,
        // 表示加载队列
        Load,
        // 表示队列的最大索引值
        Max,
    }
}
