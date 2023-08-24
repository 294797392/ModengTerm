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
    public class MouseOptions
    {
        /// <summary>
        /// 光标样式
        /// </summary>
        [JsonProperty("style")]
        public VTCursorStyles CursorStyle { get; set; }

        /// <summary>
        /// 闪烁间隔时间
        /// </summary>
        [JsonProperty("interval")]
        public int CursorInterval { get; set; }

        /// <summary>
        /// 滚轮滚动一下，滚动几行
        /// </summary>
        [JsonProperty("scrollDelta")]
        public int ScrollDelta { get; set; }
    }
}
