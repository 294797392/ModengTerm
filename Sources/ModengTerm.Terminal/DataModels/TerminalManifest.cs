using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

namespace ModengTerm.Terminal.DataModels
{
    public class TerminalManifest
    {
        /// <summary>
        /// XTerminal支持的文字大小列表
        /// </summary>
        [JsonProperty("fontSize")]
        public List<FontSizeDefinition> FontSizeList { get; private set; }

        /// <summary>
        /// 支持的文字样式列表
        /// </summary>
        [JsonProperty("fontFamily")]
        public List<FontFamilyDefinition> FontFamilyList { get; private set; }

        /// <summary>
        /// 主题列表
        /// </summary>
        [JsonProperty("defaultThemes")]
        public List<ThemePackage> DefaultThemes { get; private set; }

        public TerminalManifest()
        {
            this.FontSizeList = new List<FontSizeDefinition>();
            this.FontFamilyList = new List<FontFamilyDefinition>();
            this.DefaultThemes = new List<ThemePackage>();
        }
    }
}
