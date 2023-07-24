using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.Enumerations;

namespace XTerminal.Base.DataModels.Session
{
    /// <summary>
    /// 光标选项
    /// </summary>
    public class CursorOptions
    {
        /// <summary>
        /// 光标样式
        /// </summary>
        [JsonProperty("style")]
        public VTCursorStyles Style { get; set; }

        /// <summary>
        /// 闪烁间隔时间
        /// </summary>
        [JsonProperty("interval")]
        public int Interval { get; set; }
    }
}
