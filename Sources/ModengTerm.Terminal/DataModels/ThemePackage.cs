using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.DataModels
{
    /// <summary>
    /// 主题包
    /// </summary>
    public class ThemePackage
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
        public VTColorTable ColorTable { get; private set; }

        /// <summary>
        /// 背景类型
        /// </summary>
        [JsonProperty("backgroundType")]
        public int BackgroundType { get; set; }

        /// <summary>
        /// 背景颜色值或者是背景图片Uri
        /// </summary>
        [JsonProperty("backgroundUri")]
        public string BackgroundUri { get; set; }

        /// <summary>
        /// 背景主颜色
        /// </summary>
        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 背景特效
        /// </summary>
        [JsonProperty("backgroundEffect")]
        public int BackgroundEffect { get; set; }

        /// <summary>
        /// 默认前景色
        /// 格式是r,g,b
        /// </summary>
        [JsonProperty("foregroundColor")]
        public string ForegroundColor { get; set; }

        public ThemePackage()
        {
        }
    }
}
