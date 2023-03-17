using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public class VTMarginedDocument : VTDocumentBase
    {
        /// <summary>
        /// 所属文档
        /// </summary>
        public VTDocument OwnerDocument { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">该页边距文档所属的文档</param>
        public VTMarginedDocument(VTDocument owner)
        {
            this.OwnerDocument = owner;
        }

        #region 实例方法

        /// <summary>
        /// 向末尾追加n行
        /// </summary>
        /// <param name="lines"></param>
        private void AppendLines(int lines)
        {
            VTextLine previousLine = this.LastLine;

            for (int i = 0; i < lines; i++)
            {
                VTextLine line = new VTextLine(this);

                line.PreviousLine = previousLine;
                previousLine.NextLine = line;
                previousLine = line;
                this.LastLine = line;
            }

            this.IsArrangeDirty = true;
        }

        #endregion

        /// <summary>
        /// 向该文档里追加多个新行
        /// </summary>
        /// <param name="lines">要追加的行数</param>
        public void InitializeLines(int lines)
        {
            this.FirstLine = new VTextLine(this);
            this.LastLine = this.FirstLine;

            // 剩余要创建的行数
            int num = lines - 1;

            this.AppendLines(num);

            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 从文档结尾移除N行
        /// </summary>
        /// <param name="lines"></param>
        public void Remove(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                this.LastLine = this.LastLine.PreviousLine;
            }

            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 向文档结尾追加N行
        /// </summary>
        public void Add(int lines)
        {
            this.AppendLines(lines);

            this.IsArrangeDirty = true;
        }
    }
}
