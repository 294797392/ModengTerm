using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Parser;

namespace XTerminal.Document
{
    public class VTextStyle
    {
        /// <summary>
        /// 默认Typeface
        /// </summary>
        public static readonly VTextStyle Default = new VTextStyle() { FontFamily = "宋体", FontSize = 14, Foreground = VTForeground.DarkBlack };

        private string hashId;

        public string HashID
        {
            get
            {
                if(string.IsNullOrEmpty(this.hashId))
                {
                    this.hashId = string.Format("{0}_{1}", this.FontFamily, this.FontWeight);
                }
                return this.hashId;
            }
        }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体粗细
        /// </summary>
        public VFontWeight FontWeight { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// 文本的颜色
        /// </summary>
        public VTForeground Foreground { get; set; }
    }
}
