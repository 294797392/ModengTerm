using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 定义文档的渲染接口
    /// </summary>
    public interface IDocumentRenderer
    {
        /// <summary>
        /// 初始化渲染器
        /// </summary>
        /// <param name="options"></param>
        void Initialize(DocumentRendererOptions options);

        /// <summary>
        /// 获取行的渲染对象
        /// </summary>
        /// <returns></returns>
        List<IDocumentDrawable> GetDrawableLines();

        /// <summary>
        /// 获取光标的渲染对象
        /// </summary>
        /// <returns></returns>
        IDocumentDrawable GetDrawableCursor();

        /// <summary>
        /// 测量某个文本块的属性
        /// </summary>
        /// <param name="text">要测量的文本</param>
        /// <param name="style">文本的样式</param>
        /// <returns></returns>
        VTElementMetrics MeasureText(string text, VTextStyle style);

        /// <summary>
        /// 重新调整终端大小
        /// </summary>
        /// <param name="width">终端的宽度</param>
        /// <param name="height">终端高度</param>
        void Resize(double width, double height);
    }
}
