using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalRender
{
    public class VTextBlock
    {
        /// <summary>
        /// 要显示的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 该文本块左上角的X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 该文本块左上角的Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 渲染后该文本块的宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 渲染后该文本块的高度
        /// </summary>
        public double Height { get; set; }
    }
}
