using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using Newtonsoft.Json;
using System.Collections.Generic;
using WPFToolkit.MVVM;

namespace ModengTerm.Base
{
    public class VTManifest : AppManifest
    {
        /// <summary>
        /// 支持的会话列表
        /// </summary>
        [JsonProperty("sessions")]
        public List<SessionDefinition> SessionList { get; private set; }

        /// <summary>
        /// 默认要打开的会话
        /// </summary>
        [JsonProperty("defaultSession")]
        public XTermSession DefaultSession { get; set; }

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
        /// 终端类型的参数树形列表
        /// </summary>
        [JsonProperty("termOptionMenu")]
        public List<MenuDefinition> TerminalOptionMenu { get; private set; }

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

        /// <summary>
        /// 插件列表
        /// </summary>
        [JsonProperty("addons")]
        public List<AddonDefinition> Addons { get; private set; }


        public VTManifest()
        {
            this.SessionList = new List<SessionDefinition>();
            this.ToolbarMenus = new List<AddonMenuDefinition>();
            this.AppThemes = new List<AppTheme>();
            this.TerminalOptionMenu = new List<MenuDefinition>();
            this.FontSizeList = new List<FontSizeDefinition>();
            this.FontFamilyList = new List<FontFamilyDefinition>();
            this.DefaultThemes = new List<ThemePackage>();
            this.Addons = new List<AddonDefinition>();
        }
    }

}
