using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 终端显示屏
    /// 显示屏里包含多个Surface，Surface用来真正渲染终端输出
    /// </summary>
    public interface ITerminalScreen
    {
        /// <summary>
        /// 当用户按下按键的时候要触发这个事件
        /// IVideoTerminal：触发事件的VideoTerminal
        /// VTKeys：用户按下的键盘按键
        /// VTModifierKeys：用户按下的控制按键（ctrl，alt...etc）
        /// string：用户输入的中文字符串，如果没有则写null
        /// </summary>
        event Action<ITerminalScreen, VTInputEvent> InputEvent;

        /// <summary>
        /// 当用户拖动滚动条的时候触发
        /// int:滚动条移动到的行数
        /// </summary>
        event Action<ITerminalScreen, int> ScrollChanged;

        /// <summary>
        /// 鼠标移动的时候触发
        /// 在鼠标移动出Panel外的时候，也会触发
        /// </summary>
        event Action<ITerminalScreen, VTPoint> VTMouseMove;

        /// <summary>
        /// 鼠标按下的时候触发
        /// </summary>
        event Action<ITerminalScreen, VTPoint> VTMouseDown;

        /// <summary>
        /// 鼠标抬起的时候触发
        /// </summary>
        event Action<ITerminalScreen, VTPoint> VTMouseUp;

        /// <summary>
        /// 鼠标滚轮滚动的时候触发
        /// 如果向上滚动则为true，否则为false
        /// </summary>
        event Action<ITerminalScreen, bool> VTMouseWheel;

        /// <summary>
        /// 创建一个画板
        /// </summary>
        /// <returns></returns>
        ITerminalSurface CreateSurface();

        /// <summary>
        /// 切换Canvas
        /// </summary>
        /// <param name="remove">要移除的canvas</param>
        /// <param name="add">要显示的canvas</param>
        void SwitchSurface(ITerminalSurface remove, ITerminalSurface add);

        /// <summary>
        /// 把画布加到容器里
        /// </summary>
        /// <param name="canvas"></param>
        void AddSurface(ITerminalSurface canvas);

        /// <summary>
        /// 更新滚动条的信息
        /// </summary>
        /// <param name="maximum">滚动条的最大值</param>
        void UpdateScrollInfo(int maximum);

        /// <summary>
        /// 把滚动条滚动到底
        /// </summary>
        /// <param name="orientation">滚动的方向</param>
        void ScrollToEnd(ScrollOrientation orientation);

        /// <summary>
        /// 滚动到某个值
        /// </summary>
        /// <param name="scrollValue"></param>
        void ScrollTo(int scrollValue);
    }
}
