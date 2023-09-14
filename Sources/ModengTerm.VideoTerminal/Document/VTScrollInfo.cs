using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 存储滚动条的信息
    /// </summary>
    public class VTScrollInfo
    {
        private bool dirty;
        private int scrollMax;
        private int scrollValue;

        public bool Dirty { get { return this.dirty; } }

        /// <summary>
        /// 可以滚动到的最大值
        /// 也就是滚动条滚动到底的时候，滚动条的值
        /// </summary>
        public int ScrollMax 
        {
            get { return this.scrollMax; }
            set
            {
                if (this.scrollMax != value)
                {
                    this.scrollMax = value;
                    this.SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 滚动条的值
        /// 也就是当前Document上渲染的第一行的PhysicsRow
        /// 默认值是0
        /// </summary>
        public int ScrollValue 
        {
            get { return this.scrollValue; }
            set
            {
                if (this.scrollValue != value)
                {
                    this.scrollValue = value;
                    this.SetDirty(true);
                }
            }
        }

        internal void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;
            }
        }
    }
}
