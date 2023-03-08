using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Document
{
    public enum ScrollOrientation
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// 定义终端显示器的接口
    /// </summary>
    public interface IDrawingCanvas
    {
        /// <summary>
        /// 渲染一行
        /// </summary>
        /// <param name="textLine"></param>
        void DrawLine(VTextLine textLine);

        /// <summary>
        /// 测量某个文本块的属性
        /// </summary>
        /// <param name="text">要测量的文本</param>
        /// <param name="style">文本的样式</param>
        /// <returns></returns>
        VTextMetrics MeasureText(string text, VTextStyle style);

        /// <summary>
        /// 重新调整终端大小
        /// </summary>
        /// <param name="width">终端的宽度</param>
        /// <param name="height">终端高度</param>
        void Resize(double width, double height);

        /// <summary>
        /// 滚动到某个方向的最底部
        /// </summary>
        void ScrollToEnd(ScrollOrientation direction);
    }
}
