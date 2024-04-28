using CommandLine; // 引入CommandLine库，用于解析命令行参数
using System; // 引入System命名空间，包含一些基本的系统类型和操作
using System.Collections.Generic; // 引入System.Collections.Generic命名空间，包含泛型集合类的定义

namespace ET // 命名空间ET，代表Egametech框架
{
    // 应用类型枚举
    public enum AppType
    {
        Server, // 服务器
        Watcher, // 观察者，用于启动物理机上的所有进程
        GameTool, // 游戏工具
        ExcelExporter, // Excel导出器
        Proto2CS, // Proto2CS代码生成器
        BenchmarkClient, // 基准测试客户端
        BenchmarkServer, // 基准测试服务器
    }

    // 命令行参数类
    public class Options : Singleton<Options> // 继承自Singleton<Options>类，实现单例模式
    {
        // 应用类型选项，用于指定应用类型，默认值为AppType.Server
        [Option("AppType", Required = false, Default = AppType.Server, HelpText = "AppType enum")]
        public AppType AppType { get; set; }

        // 启动配置选项，用于指定启动配置文件的路径，默认为"StartConfig/Localhost"
        [Option("StartConfig", Required = false, Default = "StartConfig/Localhost")]
        public string StartConfig { get; set; }

        // 进程数选项，用于指定要启动的进程数，默认为1
        [Option("Process", Required = false, Default = 1)]
        public int Process { get; set; }

        // 开发模式选项，用于指定开发模式，0表示正式模式，1表示开发模式，2表示压测模式，默认为0
        [Option("Develop", Required = false, Default = 0, HelpText = "develop mode, 0正式 1开发 2压测")]
        public int Develop { get; set; }

        // 日志级别选项，用于指定日志级别，默认为2
        [Option("LogLevel", Required = false, Default = 2)]
        public int LogLevel { get; set; }

        // 控制台输出选项，用于控制是否输出到控制台，默认为0
        [Option("Console", Required = false, Default = 0)]
        public int Console { get; set; }

        // 进程启动是否创建该进程的scenes选项，用于控制是否创建场景，默认为1
        [Option("CreateScenes", Required = false, Default = 1)]
        public int CreateScenes { get; set; }
    }
}
