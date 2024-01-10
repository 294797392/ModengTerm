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
    /// 显示屏里包含多个Canvas，Canvas用来真正渲染终端输出
    /// </summary>
    public interface IDrawingTerminal
    {
        /// <summary>
        /// 创建一个画板用来渲染文档
        /// </summary>
        /// <param name="canvasIndex">要新建的Canvas在界面上的索引</param>
        /// <returns>画板实例</returns>
        IDrawingCanvas CreateCanvas(int canvasIndex);

        /// <summary>
        /// 删除一个画板
        /// </summary>
        /// <param name="canvas"></param>
        void DeleteCanvas(IDrawingCanvas canvas);

        /// <summary>
        /// 显示或者隐藏文档
        /// </summary>
        /// <param name="canvas"></param>
        void VisibleCanvas(IDrawingCanvas canvas, bool visible);

        /// <summary>
        /// 获取字形信息
        /// </summary>
        /// <param name="textStyle">字体样式</param>
        /// <returns></returns>
        VTypeface GetTypeface(double fontSize, string fontFamily);

        /// <summary>
        /// 获取整个终端窗口的大小
        /// </summary>
        /// <returns></returns>
        VTSize GetSize();
    }
}
