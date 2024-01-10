using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 定义脏区域
    /// </summary>
    public enum VTDirtyFlags
    {
        /// <summary>
        /// 可见性改变了
        /// </summary>
        VisibleDirty,

        /// <summary>
        /// 大小改变
        /// </summary>
        SizeDirty,

        /// <summary>
        /// 位置改变了
        /// </summary>
        PositionDirty,

        /// <summary>
        /// 重绘
        /// 元素是否需要重新渲染
        /// 对于VTextLine来说，Render分两步，第一步是对文字进行排版，第二部是画，排版操作是很耗时的
        /// Render的同时也会进行Measure操作
        /// </summary>
        RenderDirty,
    }
}
