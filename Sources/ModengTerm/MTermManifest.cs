using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;

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

        public MTermManifest()
        {
            this.SessionList = new List<SessionDefinition>();
        }
    }
}
