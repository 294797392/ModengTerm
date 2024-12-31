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
        /// 背景图片URI
        /// </summary>
        [JsonProperty("backImageUri")]
        public string BackImageUri { get; set; }

        /// <summary>
        /// 背景图片透明度
        /// </summary>
        [JsonProperty("backImageOpacity")]
        public double BackImageOpacity { get; set; }

        /// <summary>
        /// 背景主颜色
        /// </summary>
        [JsonProperty("backColor")]
        public string BackColor { get; set; }

        /// <summary>
        /// 默认前景色
        /// 格式是r,g,b
        /// </summary>
        [JsonProperty("fontColor")]
        public string FontColor { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        [JsonProperty("fontSize")]
        public int FontSize { get; set; }

        [JsonProperty("highlightFontColor")]
        public string HighlightFontColor { get; set; }

        [JsonProperty("highlightBackColor")]
        public string HighlightBackColor { get; set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        [JsonProperty("cursorColor")]
        public string CursorColor { get; set; }

        [JsonProperty("scrollbarThumbColor")]
        public string ScrollbarThumbColor { get; set; }

        [JsonProperty("scrollbarButtonColor")]
        public string ScrollbarButtonColor { get; set; }

        [JsonProperty("scrollbarTrackColor")]
        public string ScrollbarTrackColor { get; set; }

        /// <summary>
        /// 书签颜色
        /// </summary>
        [JsonProperty("bookmarkColor")]
        public string BookmarkColor { get; set; }

        /// <summary>
        /// 文本选中的颜色
        /// </summary>
        [JsonProperty("selectionColor")]
        public string SelectionColor { get; set; }

        public ThemePackage()
        {
        }
    }
}
