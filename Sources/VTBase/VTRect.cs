using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalBase
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

        public VTRect(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}
