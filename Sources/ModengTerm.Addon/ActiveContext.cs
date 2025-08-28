using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Base.Metadatas;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 保存插件激活时候的上下文信息
    /// </summary>
    public class ActiveContext
    {
        public ClientFactory Factory { get; set; }

        public IClient HostWindow { get; set; }

        /// <summary>
        /// 存储服务
        /// </summary>
        public IClientStorage StorageService { get; set; }

        public AddonMetadata Definition { get; set; }
    }
}
