using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Metadatas;
using Newtonsoft.Json;
using System.Collections.Generic;
using WPFToolkit.MVVM;

namespace ModengTerm.Base
{
    public class ClientManifest : AppManifest
    {
        /// <summary>
        /// 窗口主题列表
        /// </summary>
        [JsonProperty("themes")]
        public List<AppTheme> AppThemes { get; private set; }

        /// <summary>
        /// 插件列表
        /// </summary>
        [JsonProperty("addons")]
        public List<AddonMetadata> Addons { get; private set; }

        public ClientManifest()
        {
            this.AppThemes = new List<AppTheme>();
            this.Addons = new List<AddonMetadata>();
        }
    }
}
