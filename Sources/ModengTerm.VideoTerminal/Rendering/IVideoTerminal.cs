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
    public interface IVideoTerminal
    {
        /// <summary>
        /// 创建一个画板用来渲染文档
        /// </summary>
        /// <param name="options">渲染选项</param>
        /// <returns></returns>
        IDrawingCanvas CreateCanvas();

        void AddCanvas(IDrawingCanvas canvas);

        void RemoveCanvas(IDrawingCanvas canvas);

        /// <summary>
        /// 获取滚动条信息
        /// </summary>
        /// <param name="maximum">滚动条最大值</param>
        /// <param name="scrollValue">当前滚动条的值</param>
        /// <returns></returns>
        void GetScrollInfo(out int maximum, out int scrollValue);

        /// <summary>
        /// 更新滚动条的信息
        /// </summary>
        /// <param name="maximum">滚动条的最大值</param>
        void SetScrollInfo(int maximum, int scrollValue);

        VTextMetrics MeasureText(string text, double fontSize, string fontFamily);
    }
}
