using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    /// <summary>
    /// 终端主题配置
    /// </summary>
    public class Theme
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// SGR颜色表
        /// ColorName -> r,g,b
        /// </summary>
        [JsonProperty("colorTable")]
        public Dictionary<string, string> ColorTable { get; private set; }

        /// <summary>
        /// 背景颜色
        /// 格式是r,g,b
        /// </summary>
        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 默认前景色
        /// 格式是r,g,b
        /// </summary>
        [JsonProperty("foregroundColor")]
        public string ForegroundColor { get; set; }

        public Theme()
        {
            this.ColorTable = new Dictionary<string, string>();
        }
    }
}
