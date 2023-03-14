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
        /// 画
        /// 如果是文本元素，将对文本进行重新排版并渲染
        /// 排版是比较耗时的操作
        /// </summary>
        /// <param name="drawable"></param>
        void DrawDrawable(IDocumentDrawable drawable);

        /// <summary>
        /// 更新元素的位置信息
        /// 而不用重新画，速度要比DrawDrawable快
        /// 画文本的速度还是比较慢的，因为需要对文本进行排版，耗时都花在排版上面了
        /// 所以能不排版就最好不排版
        /// </summary>
        /// <param name="drawable"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        void UpdatePosition(IDocumentDrawable drawable, double offsetX, double offsetY);

        /// <summary>
        /// 设置元素的透明度
        /// </summary>
        /// <param name="drawable"></param>
        /// <param name="opacity"></param>
        void SetOpacity(IDocumentDrawable drawable, double opacity);

        /// <summary>
        /// 重新调整终端大小
        /// </summary>
        /// <param name="width">终端的宽度</param>
        /// <param name="height">终端高度</param>
        void Resize(double width, double height);
    }
}
