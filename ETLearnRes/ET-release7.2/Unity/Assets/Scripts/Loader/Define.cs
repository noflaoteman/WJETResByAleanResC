namespace ET
{
    // 定义了一些常量和方法，用于控制和配置游戏的一些特性
    public static class Define
    {
        // 游戏编译输出目录
        public const string BuildOutputDir = "./Temp/Bin/Debug";

        // 判断是否在Unity编辑器下且不使用异步操作
#if UNITY_EDITOR && !ASYNC
        public static bool IsAsync = false;
#else
        public static bool IsAsync = true;
#endif

        // 判断是否在Unity编辑器下
#if UNITY_EDITOR
        public static bool IsEditor = true;
#else
        public static bool IsEditor = false;
#endif

        // 判断是否启用代码热更新
#if ENABLE_CODES
        public static bool EnableCodes = true;
#else
        public static bool EnableCodes = false;
#endif

        // 判断是否启用视图
#if ENABLE_VIEW
        public static bool EnableView = true;
#else
        public static bool EnableView = false;
#endif

        // 判断是否启用IL2CPP
#if ENABLE_IL2CPP
        public static bool EnableIL2CPP = true;
#else
        public static bool EnableIL2CPP = false;
#endif

        // 在Unity编辑器中根据路径加载资源
        public static UnityEngine.Object LoadAssetAtPath(string s)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(s);
#else
            return null;
#endif
        }

        // 在Unity编辑器中根据AssetBundle名称获取包含的资源路径数组
        public static string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
#else
            return new string[0];
#endif
        }

        // 在Unity编辑器中根据AssetBundle名称获取依赖的AssetBundle名称数组
        public static string[] GetAssetBundleDependencies(string assetBundleName, bool v)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetBundleDependencies(assetBundleName, v);
#else
            return new string[0];
#endif
        }
    }
}
