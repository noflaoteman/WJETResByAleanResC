namespace ET
{
    // 定义场景类型枚举
    public enum SceneType
    {
        // 未定义的场景类型
        None = -1,
        // 进程场景
        Process = 0,
        // 管理器场景
        Manager = 1,
        // 领域场景
        Realm = 2,
        // 网关场景
        Gate = 3,
        // HTTP场景
        Http = 4,
        // 定位场景
        Location = 5,
        // 地图场景
        Map = 6,
        // 路由器场景
        Router = 7,
        // 路由器管理器场景
        RouterManager = 8,
        // 机器人场景
        Robot = 9,
        // 基准客户端场景
        BenchmarkClient = 10,
        // 基准服务器场景
        BenchmarkServer = 11,
        // 基准场景
        Benchmark = 12,

        // 客户端模型层场景
        Client = 31,
        // 当前场景
        Current = 34,
    }
}
