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

    public enum VTMouseButton
    { 
        LeftButton = 0,
        RightButton = 1,
        MiddleButton = 2,
    }

    public enum VTModifierKeys
    { 
        /// <summary>
        /// 没有任何按键按下
        /// </summary>
        None = 0,

        /// <summary>
        /// Shift键
        /// </summary>
        Shift = 4,

        /// <summary>
        /// 在老式的终端设备上叫Meta键，现在是Alt键
        /// </summary>
        MetaAlt = 8,

        /// <summary>
        /// Control键
        /// </summary>
        Control = 16
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

        /// <summary>
        /// 标识哪个按钮触发的事件
        /// </summary>
        public VTMouseButton Button { get; set; }
    }

    public class ScrollChangedData
    {
        public int OldScroll { get; set; }

        public int NewScroll { get; set; }
    }
}
