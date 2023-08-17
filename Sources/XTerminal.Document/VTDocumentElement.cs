using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 指定文档上的元素类型
    /// </summary>
    public enum VTDocumentElements
    {
        TextLine,

        Cursor,

        SelectionRange
    }

    /// <summary>
    /// 表示文档上的一个元素
    /// </summary>
    public abstract class VTDocumentElement
    {
        /// <summary>
        /// 该对象的类型
        /// </summary>
        public abstract VTDocumentElements Type { get; }

        /// <summary>
        /// 用来保存不同平台的绘图上下文信息
        /// </summary>
        public object DrawingContext { get; set; }

        /// <summary>
        /// 该元素左上角的X坐标
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// 该元素左上角的Y坐标
        /// </summary>
        public double OffsetY { get; set; }

        public VTDocumentElement()
        {
        }
    }
}
