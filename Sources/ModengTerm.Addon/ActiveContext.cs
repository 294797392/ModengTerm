using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    /// <summary>
    /// 保存插件激活时候的上下文信息
    /// </summary>
    public class ActiveContext
    {
        public ClientFactory Factory { get; set; }

        public IClientWindow HostWindow { get; set; }

        /// <summary>
        /// 存储服务
        /// </summary>
        public StorageService StorageService { get; set; }

        public AddonDefinition Definition { get; set; }
    }
}
