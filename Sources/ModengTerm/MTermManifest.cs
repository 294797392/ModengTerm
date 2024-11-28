using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModengTerm
{
    public class MTermManifest : AppManifest
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
        /// 窗口主题列表
        /// </summary>
        [JsonProperty("themes")]
        public List<AppTheme> AppThemes { get; private set; }

        [JsonProperty("termOptionMenu")]
        public List<OptionDefinition> TerminalOptionMenu { get; private set; }

        public MTermManifest()
        {
            this.SessionList = new List<SessionDefinition>();
            this.AppThemes = new List<AppTheme>();
            this.TerminalOptionMenu = new List<OptionDefinition>();
        }
    }
}
