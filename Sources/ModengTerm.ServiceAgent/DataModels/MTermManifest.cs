using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Terminal.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

namespace ModengTerm.ServiceAgents
{
    public class MTermManifest : AppManifest
    {
        /// <summary>
        /// 支持的会话列表
        /// </summary>
        [JsonProperty("sessions")]
        public List<SessionDefinition> SessionList { get; private set; }

        /// <summary>
        /// 定义ModengTerm支持的默认颜色
        /// </summary>
        [JsonProperty("colorList")]
        public List<ColorDefinition> ColorList { get; private set; }

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
        [JsonProperty("themes")]
        public List<Theme> ThemeList { get; private set; }

        [JsonProperty("ftpOptions")]
        public List<OptionDefinition> FTPOptionList { get; private set; }

        [JsonProperty("terminalOptions")]
        public List<OptionDefinition> TerminalOptionList { get; private set; }

        public MTermManifest()
        {
            this.SessionList = new List<SessionDefinition>();
            this.ColorList = new List<ColorDefinition>();
            this.FontSizeList = new List<FontSizeDefinition>();
            this.FontFamilyList = new List<FontFamilyDefinition>();
            this.ThemeList = new List<Theme>();
            this.FTPOptionList = new List<OptionDefinition>();
            this.TerminalOptionList = new List<OptionDefinition>();
        }
    }
}
