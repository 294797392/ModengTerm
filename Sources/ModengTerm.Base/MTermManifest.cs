using DotNEToolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

namespace ModengTerm.Base
{
    public class MTermManifest : AppManifest
    {
        /// <summary>
        /// 支持的会话列表
        /// </summary>
        [JsonProperty("sessions")]
        public List<SessionDefinition> SessionList { get; private set; }

        /// <summary>
        /// 定义ModengTerm支持的文本颜色
        /// </summary>
        [JsonProperty("foreground")]
        public List<ColorDefinition> ForegroundList { get; private set; }

        /// <summary>
        /// 定义ModengTerm支持的背景颜色
        /// </summary>
        [JsonProperty("background")]
        public List<ColorDefinition> BackgroundList { get; private set; }

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
        public List<ThemeDefinition> ThemeList { get; private set; }

        [JsonProperty("ftpOptions")]
        public List<OptionDefinition> FTPOptionList { get; private set; }

        [JsonProperty("terminalOptions")]
        public List<OptionDefinition> TerminalOptionList { get; private set; }

        public MTermManifest()
        {
            this.SessionList = new List<SessionDefinition>();
            this.ForegroundList = new List<ColorDefinition>();
            this.BackgroundList = new List<ColorDefinition>();
            this.FontSizeList = new List<FontSizeDefinition>();
            this.FontFamilyList = new List<FontFamilyDefinition>();
            this.ThemeList = new List<ThemeDefinition>();
            this.FTPOptionList = new List<OptionDefinition>();
            this.TerminalOptionList = new List<OptionDefinition>();
        }
    }
}
