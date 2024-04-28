using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ET
{
    /// <summary>
    /// 代码加载器，用于加载程序集和热重载
    /// </summary>
    public class CodeLoader : Singleton<CodeLoader>
    {
        private Assembly model; // 存储模型的程序集

        /// <summary>
        /// 开始加载代码
        /// </summary>
        public void Start()
        {
            // 如果启用了代码模式
            if (Define.EnableCodes)
            {
                // 加载全局配置
                GlobalConfig globalConfig = Resources.Load<GlobalConfig>("GlobalConfig");
                // 检查代码模式是否为客户端服务器模式
                if (globalConfig.CodeMode != CodeMode.ClientServer)
                {
                    throw new Exception("ENABLE_CODES mode must use ClientServer code mode!");
                }

                // 获取当前域中的所有程序集
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                // 获取所有程序集中的类型
                Dictionary<string, Type> typeDic = AssemblyHelper.GetAssemblyTypes(assemblies);
                // 将类型添加到事件系统中
                EventSystem.Instance.Add(typeDic);
                // 遍历所有程序集
                foreach (Assembly ass in assemblies)
                {
                    // 获取程序集的名称
                    string name = ass.GetName().Name;
                    // 如果是 Unity.Model.Codes 程序集，则将其赋值给模型
                    if (name == "Unity.Model.Codes")
                    {
                        this.model = ass;
                    }
                }
            }
            else // 如果没有启用代码模式
            {
                byte[] assBytes;
                byte[] pdbBytes;
                // 如果不是在编辑器中
                if (!Define.IsEditor)
                {
                    // 从资源包加载程序集和调试信息
                    Dictionary<string, UnityEngine.Object> dictionary = AssetsBundleHelper.LoadBundle("code.unity3d");
                    assBytes = ((TextAsset)dictionary["Model.dll"]).bytes;
                    pdbBytes = ((TextAsset)dictionary["Model.pdb"]).bytes;

                    // 如果启用了 IL2CPP
                    if (Define.EnableIL2CPP)
                    {
                        // 加载混合 CLR
                        HybridCLRHelper.Load();
                    }
                }
                else // 如果在编辑器中
                {
                    // 从输出目录加载程序集和调试信息
                    assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Model.dll"));
                    pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, "Model.pdb"));
                }

                // 加载模型程序集
                this.model = Assembly.Load(assBytes, pdbBytes);
                // 加载热更代码
                this.LoadHotfix();
            }

            // 获取 ET.Entry.Start 方法
            IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
            // 执行 ET.Entry.Start 方法
            start.Run();
        }

        /// <summary>
        /// 加载热更代码
        /// </summary>
        public void LoadHotfix()
        {
            byte[] assBytes;
            byte[] pdbBytes;
            // 如果不在编辑器中
            if (!Define.IsEditor)
            {
                // 从资源包加载程序集和调试信息
                Dictionary<string, UnityEngine.Object> dictionary = AssetsBundleHelper.LoadBundle("code.unity3d");
                assBytes = ((TextAsset)dictionary["Hotfix.dll"]).bytes;
                pdbBytes = ((TextAsset)dictionary["Hotfix.pdb"]).bytes;
            }
            else // 如果在编辑器中
            {
                // 获取编辑器输出目录中的热更代码文件
                string[] logicFiles = Directory.GetFiles(Define.BuildOutputDir, "Hotfix_*.dll");
                if (logicFiles.Length != 1)
                {
                    throw new Exception("Logic dll count != 1");
                }
                string logicName = Path.GetFileNameWithoutExtension(logicFiles[0]);
                assBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.dll"));
                pdbBytes = File.ReadAllBytes(Path.Combine(Define.BuildOutputDir, $"{logicName}.pdb"));
            }

            // 加载热更程序集
            Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);

            // 获取所有程序集中的类型
            Dictionary<string, Type> typesDic = AssemblyHelper.GetAssemblyTypes(typeof(Game).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly);

            // 将类型添加到事件系统中
            EventSystem.Instance.Add(typesDic);
        }
    }
}
