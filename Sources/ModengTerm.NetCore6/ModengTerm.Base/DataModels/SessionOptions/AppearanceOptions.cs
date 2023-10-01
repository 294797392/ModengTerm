using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.DataModels.SessionOptions
{
    /// <summary>
    /// 外观设置
    /// </summary>
    public class AppearanceOptions
    {
        /// <summary>
        /// 外观ID
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 外观名字
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 字体样式
        /// </summary>
        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        [JsonProperty("fontSize")]
        public int FontSize { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        [JsonProperty("foreground")]
        public string Foreground { get; set; }
    }
}
