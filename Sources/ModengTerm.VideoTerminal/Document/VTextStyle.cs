using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
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

        /// <summary>
        /// 字体大小
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// 终端的前景色
        /// </summary>
        public string Foreground { get; set; }

        /// <summary>
        /// 终端的背景色
        /// </summary>
        public Wallpaper Background { get; set; }

        /// <summary>
        /// 颜色表
        /// </summary>
        public VTColorTable ColorTable { get; set; }

        public VTextStyle() 
        {
        }
    }
}
