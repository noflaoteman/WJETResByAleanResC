using System; // 引入 System 命名空间
using System.Collections.Generic; // 引入 System.Collections.Generic 命名空间
using System.Reflection; // 引入 System.Reflection 命名空间

namespace ET // 定义 ET 命名空间
{
    public static class AssemblyHelper // 定义一个静态类 AssemblyHelper
    {
        // 获取程序集中的所有类型，并以类型全名为键，类型对象为值的字典形式返回
        public static Dictionary<string, Type> GetAssemblyTypes(params Assembly[] args)
        {
            // 创建一个空字典 typesDic 用于存储类型
            Dictionary<string, Type> typesDic = new Dictionary<string, Type>();

            // 遍历传入的所有程序集
            foreach (Assembly ass in args)
            {
                // 遍历当前程序集中的所有类型
                foreach (Type type in ass.GetTypes())
                {
                    // 将类型的全名作为键，类型对象作为值，存入字典 typesDic 中
                    typesDic[type.FullName] = type;
                }
            }

            // 返回存储了程序集中所有类型的字典
            return typesDic;
        }
    }
}
