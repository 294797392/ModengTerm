using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 定义可渲染对象的类型
    /// </summary>
    public enum Drawables
    {
        /// <summary>
        /// 一个文本行
        /// </summary>
        TextLine,

        /// <summary>
        /// 光标
        /// </summary>
        Cursor,

        /// <summary>
        /// 选项
        /// </summary>
        SelectionRange
    }

    /// <summary>
    /// 表示文档里的一个可以画的元素
    /// </summary>
    public interface VTDocumentDrawable
    {
        /// <summary>
        /// 该对象的类型
        /// </summary>
        Drawables Type { get; }

        /// <summary>
        /// 用来保存不同平台的绘图上下文信息
        /// </summary>
        object DrawingContext { get; set; }
    }
}
