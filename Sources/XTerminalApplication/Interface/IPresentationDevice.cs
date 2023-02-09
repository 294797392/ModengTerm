using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalDevice.Interface
{
    public enum ScrollOrientation
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// 表示一个用来渲染文本的设备
    /// </summary>
    public interface IPresentationDevice
    {
        /// <summary>
        /// 渲染文本块
        /// </summary>
        /// <param name="textBlocks"></param>
        void DrawText(List<VTextBlock> textBlocks);

        /// <summary>
        /// 渲染文本块
        /// </summary>
        /// <param name="textBlock"></param>
        void DrawText(VTextBlock textBlock);

        /// <summary>
        /// 测量某个文本块的属性
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        VTextBlockMetrics MeasureText(VTextBlock textBlock);

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
