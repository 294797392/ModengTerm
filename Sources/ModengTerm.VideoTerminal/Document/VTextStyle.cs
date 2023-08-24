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
        /// 字体
        /// </summary>
        public string FontFamily { get; set; }

        ///// <summary>
        ///// 字体粗细
        ///// </summary>
        //public VFontWeight FontWeight { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// 文本的颜色
        /// </summary>
        public string Foreground { get; set; }
    }
}
