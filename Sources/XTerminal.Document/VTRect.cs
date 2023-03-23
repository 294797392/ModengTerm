using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public struct VTRect
    {
        /// <summary>
        /// 左上角X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 左上角Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height { get; set; }

        public VTPoint LeftTop { get { return new VTPoint(this.X, this.Y); } }

        public VTPoint RightTop { get { return new VTPoint(this.X + this.Width, this.Y); } }

        public VTPoint LeftBottom { get { return new VTPoint(this.X, this.Y + this.Height); } }

        public VTPoint RightBottom { get { return new VTPoint(this.X + this.Width, this.Y + this.Height); } }

        /// <summary>
        /// 获取该矩形上边的Y坐标
        /// </summary>
        public double Top { get { return this.Y; } }

        /// <summary>
        /// 获取该矩形下边的Y坐标
        /// </summary>
        public double Bottom { get { return this.Top + this.Height; } }

        /// <summary>
        /// 获取该矩形左边的X坐标
        /// </summary>
        public double Left { get { return this.LeftBottom.X; } }

        /// <summary>
        /// 获取该矩形右边的X坐标
        /// </summary>
        public double Right { get { return this.Left + this.Width; } }

        public VTRect(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}
