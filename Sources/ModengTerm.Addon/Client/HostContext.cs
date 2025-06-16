using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Client
{
    /// <summary>
    /// 存储客户端的上下文信息
    /// </summary>
    public class HostContext
    {
        public IHostWindow HostWindow { get; set; }

        public StorageService StorageService { get; set; }
    }
}
