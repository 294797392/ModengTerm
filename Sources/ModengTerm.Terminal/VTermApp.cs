using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.ServiceAgents;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public class VTermAppManifest : AppManifest
    { }

    /// <summary>
    /// 存储终端相关的上下文信息
    /// </summary>
    public class VTermApp : ModularApp<VTermApp, VTermAppManifest>
    {
        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        protected override int OnInitialized()
        {
            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }
    }
}
