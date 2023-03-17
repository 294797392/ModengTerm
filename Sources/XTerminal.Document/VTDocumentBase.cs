using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public abstract class VTDocumentBase
    {
        private bool isArrangeDirty;

        /// <summary>
        /// 文档里的第一行
        /// </summary>
        public VTextLine FirstLine { get; internal set; }

        /// <summary>
        /// 文档里的最后一行
        /// </summary>
        public VTextLine LastLine { get; internal set; }

        /// <summary>
        /// 该文档是否是空文档
        /// </summary>
        public bool IsEmpty { get { return this.FirstLine == null && this.LastLine == null; } }

        /// <summary>
        /// 是否需要重新布局
        /// </summary>
        public bool IsArrangeDirty
        {
            get { return this.isArrangeDirty; }
            set
            {
                if (this.isArrangeDirty != value)
                {
                    this.isArrangeDirty = value;
                }
            }
        }
    }
}
