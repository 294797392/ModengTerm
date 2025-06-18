using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 描述一个文本区域相对于整个文档的位置和大小
    /// </summary>
    public struct VTextRange
    {
        public static readonly VTextRange Empty = new VTextRange();

        /// <summary>
        /// 第一个字符的索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 文本里的字符数量
        /// </summary>
        public int Length { get; set; }

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

        public VTextRange(int index, int length, double offsetX, double offsetY, double width, double height)
        {
            this.Index = index;
            this.Length = length;
            this.Left = offsetX;
            this.Top = offsetY;
            this.Width = width;
            this.Height = height;
        }

        public VTRect GetVTRect() 
        {
            return new VTRect(this.Left, this.Top, this.Width, this.Height);
        }
    }
}
