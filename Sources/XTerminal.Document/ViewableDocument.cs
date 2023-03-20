using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 描述可视区域在整个文档里的位置
    /// </summary>
    public enum ViewablePlacement
    {
        /// <summary>
        /// 可视区域再整个文档的中间
        /// </summary>
        Middle,

        /// <summary>
        /// 可视区域在整个文档的底部
        /// </summary>
        Bottom,

        /// <summary>
        /// 可视区域再整个文档的上面
        /// </summary>
        Top
    }

    /// <summary>
    /// 保存用户可以在界面上看到的文档区域
    /// 也就是要渲染到界面上的区域
    /// </summary>
    public class ViewableDocument : VTDocumentBase
    {
        #region 属性

        /// <summary>
        /// 所属的文档
        /// </summary>
        public VTDocument OwnerDocument { get; private set; }

        ///// <summary>
        ///// 可视区域的总行数
        ///// </summary>
        //public int Rows { get; internal set; }

        ///// <summary>
        ///// 可视区域的总列数
        ///// </summary>
        //public int Columns { get; internal set; }

        #endregion

        #region 构造方法

        public ViewableDocument(VTDocument ownerDocument)
        {
            this.OwnerDocument = ownerDocument;
        }

        #endregion

        #region 公开接口

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
                current.SetDirty(true);

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }

            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 删除所有行
        /// </summary>
        public void DeleteAll()
        {
            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                // 取消关联关系
                current.DeleteAll();

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }

            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 保持最后一行不变，通过改变第一行的指针缩小可视区域
        /// </summary>
        /// <param name="lines">要缩小的行</param>
        public void ShrinkTop(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                this.FirstLine = this.FirstLine.NextLine;
            }

            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 保持第一行不变，通过改变最后一行的指针缩小可视区域
        /// </summary>
        /// <param name="lines"></param>
        public void ShrinkBottom(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                this.LastLine = this.LastLine.PreviousLine;
            }

            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 保持最后一行不变，通过改变第一行的指针扩大可视区域
        /// </summary>
        /// <param name="lines">要扩大的行</param>
        public void ExpandTop(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                this.FirstLine = this.FirstLine.PreviousLine;
            }

            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 保持第一行不变，通过改变最后一行的指针扩大可视区域
        /// </summary>
        /// <param name="lines"></param>
        public void ExpandBottom(int lines)
        {
            for (int i = 0; i < lines; i++)
            {
                this.LastLine = this.LastLine.NextLine;
            }

            this.IsArrangeDirty = true;
        }

        #endregion
    }
}
