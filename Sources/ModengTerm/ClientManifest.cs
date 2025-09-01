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
        /// 默认顶部根菜单
        /// </summary>
        [JsonProperty("toolbarMenus")]
        public List<AddonMenuDefinition> ToolbarMenus { get; private set; }

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
            this.ToolbarMenus = new List<AddonMenuDefinition>();
            this.AppThemes = new List<AppTheme>();
            this.Addons = new List<AddonMetadata>();
        }
    }
}
