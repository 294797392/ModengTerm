using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public class VTEventInput
    {
        public delegate void MouseWheelDelegate(bool upper);
        public delegate void MouseDownDelegate(VTPoint location, int clickCount);

        /// <summary>
        /// 当鼠标移动的时候触发
        /// </summary>
        /// <param name="location">以文档左上角的位置为原点，相对于文档左上角的位置的鼠标坐标</param>
        public delegate void MouseMoveDelegate(VTPoint location);
        public delegate void MouseUpDelegate(VTPoint location);
        public delegate void SizeChangedDelegate(VTSize contentSize);
        public delegate void ScrollChangedDelegate(int scrollValue);
        public delegate void InputDelegate(VTInput input);

        public MouseWheelDelegate OnMouseWheel;
        public MouseDownDelegate OnMouseDown;
        public MouseMoveDelegate OnMouseMove;
        public MouseUpDelegate OnMouseUp;
        public SizeChangedDelegate OnSizeChanged;
        public ScrollChangedDelegate OnScrollChanged;

        /// <summary>
        /// 当外部模块输入的时候触发
        /// </summary>
        public InputDelegate OnInput;
    }
}
