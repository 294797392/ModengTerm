using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public struct VTSize
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public VTSize(double w, double h)
        {
            this.Width = w;
            this.Height = h;
        }
    }

    public struct VTRect
    {
        public static readonly VTRect Empty = new VTRect();

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

        /// <summary>
        /// 获取中心点X坐标
        /// </summary>
        public double CenterX { get { return this.Left + this.Width / 2; } }

        public VTRect(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// 测试是否在水平位置包含X
        /// </summary>
        /// <param name="x"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ContainsX(double x, int tolerance = 0)
        {
            if (tolerance > 0)
            {
                return x >= this.Left - tolerance && x <= this.Right + tolerance;
            }
            else
            {
                return x >= this.Left && x <= this.Right;
            }
        }

        public override string ToString()
        {
            return string.Format("x = {0}, y = {1}, width = {2}, height = {3}", this.LeftTop.X, this.LeftTop.Y, this.Width, this.Height);
        }
    }
}
