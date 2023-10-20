using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace ModengTerm.Terminal.Rendering
{
    /// <summary>
    /// 终端显示屏
    /// 显示屏里包含多个Surface，Surface用来真正渲染终端输出
    /// </summary>
    public interface IDrawingWindow
    {
        /// <summary>
        /// 创建一个画板用来渲染文档
        /// </summary>
        /// <returns></returns>
        IDrawingDocument CreateCanvas();

        void InsertCanvas(int index, IDrawingDocument canvas);

        void RemoveDocument(IDrawingDocument document);

        /// <summary>
        /// 显示或者隐藏文档
        /// </summary>
        /// <param name="canvas"></param>
        void VisibleCanvas(IDrawingDocument canvas, bool visible);

        /// <summary>
        /// 获取字形信息
        /// </summary>
        /// <param name="textStyle">字体样式</param>
        /// <returns></returns>
        VTypeface GetTypeface(VTextStyle textStyle);

        /// <summary>
        /// 获取整个终端窗口的大小
        /// </summary>
        /// <returns></returns>
        VTSize GetSize();
    }
}
