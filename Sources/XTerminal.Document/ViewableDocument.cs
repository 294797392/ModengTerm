using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 保存用户可以在界面上看到的文档区域
    /// 也就是要渲染到界面上的区域
    /// </summary>
    public class ViewableDocument
    {
        /// <summary>
        /// 是否需要重新布局
        /// </summary>
        public bool IsArrangeDirty { get; set; }

        /// <summary>
        /// 要渲染的第一行
        /// </summary>
        public VTextLine FirstLine { get; internal set; }

        /// <summary>
        /// 要渲染的最后一行
        /// </summary>
        public VTextLine LastLine { get; internal set; }

        /// <summary>
        /// 所属的文档
        /// </summary>
        public VTDocument OwnerDocument { get; internal set; }

        /// <summary>
        /// 当前光标所在的行
        /// </summary>
        public int CorsorRow { get; internal set; }

        /// <summary>
        /// 把当前可视区域的所有TextLine标记为需要重新渲染的状态
        /// 并且把ViewableDocument也标记为ArrangeDirty
        /// </summary>
        public void DirtyAll()
        {
            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                current.IsCharacterDirty = true;

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }

            this.IsArrangeDirty = true;
        }
    }
}
