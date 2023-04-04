using DotNEToolkit.Modular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

namespace XTerminal.Base.ServiceAgents
{
    /// <summary>
    /// 远程服务器代理
    /// 远程服务器的作用：
    /// 1. 管理分组和Session信息
    /// 2. 用户数据管理，用户登录
    /// </summary>
    public abstract class ServiceAgent : ModuleBase
    {
        public List<SessionDefinition> GetSessionDefinitions()
        {
            return XTermApp.Context.Manifest.SessionList;
        }
    }
}
