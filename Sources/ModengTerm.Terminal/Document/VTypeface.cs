using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 存储每种字形的信息
    /// </summary>
    public class VTypeface
    {
        /// <summary>
        /// 该字形高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 空白字符的宽度
        /// 注意每个字符的宽度可能都不是一样的
        /// 这个宽度是空白字符的宽度
        /// </summary>
        public double SpaceWidth { get; set; }

        /// <summary>
        /// 字体样式
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// 文本的背景色
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 文本的前景色
        /// </summary>
        public string ForegroundColor { get; set; }
    }
}
