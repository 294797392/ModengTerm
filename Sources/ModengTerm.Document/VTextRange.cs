using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 存储一行文本里的一段文本的位置
    /// </summary>
    public struct VTextRange
    {
        public static readonly VTextRange Empty = new VTextRange();

        /// <summary>
        /// 距离文档左边的X偏移量
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// 距离文档上边的Y偏移量
        /// </summary>
        public double Top { get; set; }

        public double Right
        {
            get { return this.Left + Width; }
        }

        public double Bottom 
        {
            get { return this.Top + this.Height; }
        }

        /// <summary>
        /// 该文本段宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 该文本段高度
        /// </summary>
        public double Height { get; set; }

        public VTextRange(double offsetX, double offsetY, double width, double height)
        {
            this.Left = offsetX;
            this.Top = offsetY;
            this.Width = width;
            this.Height = height;
        }
    }
}
