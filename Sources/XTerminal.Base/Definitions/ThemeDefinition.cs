using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Definitions
{
    /// <summary>
    /// 终端主题配置
    /// </summary>
    public class ThemeDefinition
    {
        /// <summary>
        /// 字体ID
        /// </summary>
        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小ID
        /// </summary>
        [JsonProperty("fontSize")]
        public string FontSize { get; set; }

        /// <summary>
        /// 前景色ID
        /// </summary>
        [JsonProperty("foreground")]
        public string Foreground { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        [JsonProperty("background")]
        public string Background { get; set; }
    }
}
