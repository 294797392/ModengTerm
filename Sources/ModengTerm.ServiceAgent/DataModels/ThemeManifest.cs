using ModengTerm.Terminal.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

namespace ModengTerm.ServiceAgents.DataModels
{
    public class ThemeManifest
    {
        /// <summary>
        /// 主题列表
        /// </summary>
        [JsonProperty("themes")]
        public List<Theme> ThemeList { get; private set; }

        /// <summary>
        /// 定义ModengTerm支持的默认颜色
        /// </summary>
        [JsonProperty("defaultColors")]
        public List<ColorDefinition> DefaultColors { get; private set; }

        /// <summary>
        /// 默认的动态背景列表
        /// </summary>
        [JsonProperty("defaultLivePapers")]
        public List<ColorDefinition> DefaultLivePapers { get; private set; }

        /// <summary>
        /// 默认的静态背景图片
        /// </summary>
        [JsonProperty("defaultPapers")]
        public List<ColorDefinition> DefaultPapers { get; private set; }
    }
}
