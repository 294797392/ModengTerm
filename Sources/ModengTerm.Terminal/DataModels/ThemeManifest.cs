using ModengTerm.Terminal.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

namespace ModengTerm.Terminal.DataModels
{
    public class ThemeManifest
    {
        /// <summary>
        /// 主题列表
        /// </summary>
        [JsonProperty("themes")]
        public List<ThemePackage> ThemeList { get; private set; }
    }
}
