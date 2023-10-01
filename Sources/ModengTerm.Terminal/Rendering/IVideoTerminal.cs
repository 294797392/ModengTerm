using ModengTerm.Terminal.Document;
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
        IDrawingDocument CreateDocument();

        void InsertDocument(int index, IDrawingDocument document);

        void RemoveDocument(IDrawingDocument canvas);

        /// <summary>
        /// 显示或者隐藏文档
        /// </summary>
        /// <param name="document"></param>
        void VisibleDocument(IDrawingDocument document, bool visible);

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
        /// 测量文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontFamily"></param>
        /// <returns></returns>
        VTextMetrics MeasureText(string text, double fontSize, string fontFamily);

        /// <summary>
        /// 获取终端屏幕的显示区域
        /// 相对于整个显示器的位置
        /// </summary>
        /// <returns></returns>
        VTRect GetDisplayRect();
    }
}
