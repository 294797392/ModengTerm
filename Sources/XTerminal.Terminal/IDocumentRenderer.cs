using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace XTerminal.Terminal
{
    //public enum ScrollOrientation
    //{
    //    Top,
    //    Bottom,
    //    Left,
    //    Right
    //}

    public interface IDocumentRenderer
    {
        /// <summary>
        /// 初始化渲染器
        /// </summary>
        /// <param name="options"></param>
        void Initialize(DocumentRendererOptions options);

        /// <summary>
        /// 测量某个文本块的属性
        /// </summary>
        /// <param name="text">要测量的文本</param>
        /// <param name="style">文本的样式</param>
        /// <returns></returns>
        VTextMetrics MeasureText(string text, VTextStyle style);

        /// <summary>
        /// 对每个VTextLine进行布局，并在IsCharacterDirty的时候重新渲染文本行
        /// </summary>
        /// <param name="vtDocument">要排版的文档</param>
        void RenderDocument(VTDocument vtDocument);

        /// <summary>
        /// 重新绘制任意一个视图对象
        /// </summary>
        /// <param name="drawingObject">要重绘的对象</param>
        void RenderElement(IDrawingElement drawingObject);

        /// <summary>
        /// 清除现实的内容并把状态还原
        /// </summary>
        void Reset();

        /// <summary>
        /// 重新调整终端大小
        /// </summary>
        /// <param name="width">终端的宽度</param>
        /// <param name="height">终端高度</param>
        void Resize(double width, double height);

        ///// <summary>
        ///// 滚动到某个方向的最底部
        ///// </summary>
        //void ScrollToEnd(ScrollOrientation direction);
    }
}
