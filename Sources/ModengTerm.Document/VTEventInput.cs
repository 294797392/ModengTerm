using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public class MouseWheelData
    {
        public int Delta { get; set; }
    }

    public interface IMousePosition
    {
        double X { get; set; }

        double Y { get; set; }
    }

    public class MouseData : IMousePosition
    {
        /// <summary>
        /// 鼠标的X坐标
        /// 基于DrawArea的坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 鼠标的Y坐标
        /// 基于DrawArea的坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 鼠标点击次数
        /// </summary>
        public int ClickCount { get; set; }
    }

    public class ScrollChangedData
    {
        public int OldScroll { get; set; }

        public int NewScroll { get; set; }
    }
}
