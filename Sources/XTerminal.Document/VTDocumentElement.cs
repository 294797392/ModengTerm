using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 表示文档上的一个元素
    /// </summary>
    public abstract class VTDocumentElement
    {
        /// <summary>
        /// 是否是脏数据
        /// 脏数据的话需要重新渲染
        /// </summary>
        public bool IsDirty { get; private set; }

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

        public void SetDirty(bool isDirty)
        {
            if (this.IsDirty != isDirty)
            {
                this.IsDirty = isDirty;
            }
        }
    }
}
