using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public class MouseData
    {
        public enum CaptureActions
        {
            /// <summary>
            /// 指定不做任何操作
            /// </summary>
            None,

            /// <summary>
            /// 指定捕获鼠标
            /// </summary>
            Capture,

            /// <summary>
            /// 指定释放捕获的鼠标
            /// </summary>
            ReleaseCapture
        }

        /// <summary>
        /// 鼠标的X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 鼠标的Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 鼠标点击次数
        /// </summary>
        public int ClickCount { get; set; }

        /// <summary>
        /// 设置是否需要捕获鼠标
        /// 捕获鼠标之后，即使鼠标移出了触发鼠标事件的元素所在区域，也会继续触发该元素的鼠标事件（比如鼠标移动事件）
        /// </summary>
        public CaptureActions CaptureAction { get; set; }

        /// <summary>
        /// 获取当前鼠标是否是捕获状态
        /// </summary>
        public bool IsMouseCaptured { get; private set; }

        public MouseData(double x, double y, int clickCount, bool isMouseCaptured)
        {
            X = x;
            Y = y;
            ClickCount = clickCount;
            this.IsMouseCaptured = isMouseCaptured;
            this.CaptureAction = CaptureActions.None;
        }
    }

    public class ScrollChangedData
    {
        public int OldScroll { get; set; }

        public int NewScroll { get; set; }
    }

    public class VTEventInput
    {
        public delegate void MouseWheelDelegate(bool upper);
        public delegate void MouseDownDelegate(MouseData mouseData);

        /// <summary>
        /// 当鼠标移动的时候触发
        /// </summary>
        /// <param name="location">以文档左上角的位置为原点，相对于文档左上角的位置的鼠标坐标</param>
        public delegate void MouseMoveDelegate(MouseData mouseData);
        public delegate void MouseUpDelegate(MouseData mouseData);
        public delegate void ScrollChangedDelegate(ScrollChangedData scrollData);

        public MouseWheelDelegate OnMouseWheel;
        public MouseDownDelegate OnMouseDown;
        public MouseMoveDelegate OnMouseMove;
        public MouseUpDelegate OnMouseUp;
        public ScrollChangedDelegate OnScrollChanged;
    }
}
