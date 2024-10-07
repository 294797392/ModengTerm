using DotNEToolkit;
using ModengTerm.Base.ServiceAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 存储终端相关的上下文信息
    /// </summary>
    public class VTApp : SingletonObject<VTApp>
    {
        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        public void Initialize()
        {

        }

        public void Release()
        {

        }
    }
}
