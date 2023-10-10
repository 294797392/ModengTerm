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
        /// <param name="options">渲染选项</param>
        /// <returns></returns>
        IDrawingCanvas CreateCanvas();

        void InsertCanvas(int index, IDrawingCanvas canvas);

        /// <summary>
        /// 显示或者隐藏文档
        /// </summary>
        /// <param name="canvas"></param>
        void VisibleCanvas(IDrawingCanvas canvas, bool visible);

        /// <summary>
        /// 获取滚动条信息
        /// </summary>
        /// <param name="scrollInfo">存储滚动条信息</param>
        /// <returns></returns>
        void GetScrollInfo(ref VTScrollInfo scrollInfo);

        /// <summary>
        /// 更新滚动条的信息
        /// </summary>
        /// <param name="scrollInfo">要设置的滚动条信息</param>
        void SetScrollInfo(VTScrollInfo scrollInfo);

        /// <summary>
        /// 设置滚动条是否可见
        /// </summary>
        /// <param name="visible"></param>
        void SetScrollVisible(bool visible);

        /// <summary>
        /// 获取字形信息
        /// </summary>
        /// <param name="textStyle">字体样式</param>
        /// <returns></returns>
        VTypeface GetTypeface(VTextStyle textStyle);

        /// <summary>
        /// 获取该窗口相对于桌面左上角的位置
        /// </summary>
        /// <returns></returns>
        VTRect GetDisplayRect();
    }
}
