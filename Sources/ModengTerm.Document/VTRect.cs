using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Document
{
    public struct VTSize
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public VTSize(double w, double h)
        {
            Width = w;
            Height = h;
        }

        /// <summary>
        /// 高度和宽度分别扩大offset长度
        /// </summary>
        /// <param name="offset">要扩大或者缩小的长度</param>
        /// <returns></returns>
        public VTSize Offset(double offset)
        {
            return new VTSize(Width + offset, Height + offset);
        }

        public override string ToString()
        {
            return string.Format("Width = {0}, Height = {1}", Width, Height);
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

        public VTPoint LeftTop { get { return new VTPoint(X, Y); } }

        public VTPoint RightTop { get { return new VTPoint(X + Width, Y); } }

        public VTPoint LeftBottom { get { return new VTPoint(X, Y + Height); } }

        public VTPoint RightBottom { get { return new VTPoint(X + Width, Y + Height); } }

        /// <summary>
        /// 获取该矩形上边的Y坐标
        /// </summary>
        public double Top { get { return Y; } }

        /// <summary>
        /// 获取该矩形下边的Y坐标
        /// </summary>
        public double Bottom { get { return Top + Height; } }

        /// <summary>
        /// 获取该矩形左边的X坐标
        /// </summary>
        public double Left { get { return LeftBottom.X; } }

        /// <summary>
        /// 获取该矩形右边的X坐标
        /// </summary>
        public double Right { get { return Left + Width; } }

        /// <summary>
        /// 获取中心点X坐标
        /// </summary>
        public double CenterX { get { return Left + Width / 2; } }

        public VTRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public VTRect(double x, double y, VTSize size)
        {
            X = x;
            Y = y;
            Width = size.Width;
            Height = size.Height;
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
                return x >= Left - tolerance && x <= Right + tolerance;
            }
            else
            {
                return x >= Left && x <= Right;
            }
        }

        public override string ToString()
        {
            return string.Format("x = {0}, y = {1}, width = {2}, height = {3}", LeftTop.X, LeftTop.Y, Width, Height);
        }

        public Rect GetRect()
        {
            return new Rect(X, Y, Width, Height);
        }

        public VTSize GetSize()
        {
            return new VTSize(Width, Height);
        }

        /// <summary>
        /// 保持中心点不变，向四周扩大相同的距离
        /// </summary>
        /// <param name="value">要扩大的距离</param>
        /// <returns></returns>
        public VTRect Extend(double value)
        {
            return new VTRect(X - value, Y - value, Width + value * 2, Height + value * 2);
        }
    }
}
