using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 表示画板容器
    /// 容器里可以显示多个画图
    /// </summary>
    public interface IDrawingCanvasPanel
    {
        /// <summary>
        /// 当用户按下按键的时候要触发这个事件
        /// IVideoTerminal：触发事件的VideoTerminal
        /// VTKeys：用户按下的键盘按键
        /// VTModifierKeys：用户按下的控制按键（ctrl，alt...etc）
        /// string：用户输入的中文字符串，如果没有则写null
        /// </summary>
        event Action<IDrawingCanvasPanel, VTInputEvent> InputEvent;

        /// <summary>
        /// 当用户拖动滚动条的时候触发
        /// int:滚动条移动到的行数
        /// </summary>
        event Action<IDrawingCanvasPanel, int> ScrollChanged;

        /// <summary>
        /// 鼠标移动的时候触发
        /// 在鼠标移动出Panel外的时候，也会触发
        /// </summary>
        event Action<IDrawingCanvasPanel, VTPoint> VTMouseMove;

        /// <summary>
        /// 鼠标按下的时候触发
        /// </summary>
        event Action<IDrawingCanvasPanel, VTPoint> VTMouseDown;

        /// <summary>
        /// 鼠标抬起的时候触发
        /// </summary>
        event Action<IDrawingCanvasPanel, VTPoint> VTMouseUp;

        /// <summary>
        /// 获取Panel的相对于整个显示器屏幕的边界框
        /// </summary>
        /// <returns></returns>
        VTRect GetBoundary();

        /// <summary>
        /// 创建一个画板
        /// </summary>
        /// <returns></returns>
        IDrawingCanvas CreateCanvas();

        /// <summary>
        /// 切换Canvas
        /// </summary>
        /// <param name="remove">要移除的canvas</param>
        /// <param name="add">要显示的canvas</param>
        void SwitchCanvas(IDrawingCanvas remove, IDrawingCanvas add);

        /// <summary>
        /// 把画布加到容器里
        /// </summary>
        /// <param name="canvas"></param>
        void AddCanvas(IDrawingCanvas canvas);

        /// <summary>
        /// 更新滚动信息
        /// </summary>
        /// <param name="maximum">滚动条的最大值</param>
        void UpdateScrollInfo(int maximum);

        /// <summary>
        /// 滚动到某一个历史行
        /// 默认把历史行设置为滚动之后的窗口中的第一行
        /// </summary>
        /// <param name="historyLine">要滚动到的历史行</param>
        void ScrollToHistoryLine(VTHistoryLine historyLine);

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
