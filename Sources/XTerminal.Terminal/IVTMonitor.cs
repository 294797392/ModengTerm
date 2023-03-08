using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace XTerminal.Terminal
{
    public enum ScrollOrientation
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public interface IVTMonitor
    {
        /// <summary>
        /// 渲染一行
        /// </summary>
        /// <param name="textLine"></param>
        void DrawLine(VTextLine textLine);

        /// <summary>
        /// 清除当前所有画的东西并渲染一个文档
        /// 该方法会自动处理滚动条和视图大小
        /// </summary>
        /// <param name="document"></param>
        void DrawDocument(VTDocument document);

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
