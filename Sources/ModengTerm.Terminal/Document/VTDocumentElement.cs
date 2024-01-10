using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 指定文档上的元素类型
    /// </summary>
    public enum VTDocumentElements
    {
        /// <summary>
        /// 文本行
        /// </summary>
        TextLine,

        /// <summary>
        /// 光标
        /// </summary>
        Cursor,

        /// <summary>
        /// 选中区域
        /// </summary>
        SelectionRange,

        /// <summary>
        /// 滚动条
        /// </summary>
        Scrollbar,

        /// <summary>
        /// 匹配的行
        /// </summary>
        MatchesLine,

        /// <summary>
        /// 背景
        /// </summary>
        Wallpaper,
    }

    /// <summary>
    /// 表示文档上的一个元素
    /// </summary>
    public abstract class VTDocumentElement<TDrawingObject> : VTElement<TDrawingObject>
        where TDrawingObject : IDrawingObject
    {
        #region 实例变量

        protected VTDocument ownerDocument;

        #endregion

        #region 属性

        /// <summary>
        /// 该行所属的文档
        /// </summary>
        public VTDocument OwnerDocument { get { return this.ownerDocument; } }

        #endregion

        #region 构造方法

        public VTDocumentElement(VTDocument ownerDocument) :
            base(ownerDocument.DrawingObject)
        {
            this.ownerDocument = ownerDocument;
        }

        #endregion
    }
}
