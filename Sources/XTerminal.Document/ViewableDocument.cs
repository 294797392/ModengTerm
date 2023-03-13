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
        private VTDocumentOptions options;

        private bool isArrangeDirty;

        #region 属性

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

        /// <summary>
        /// 可视区域的第一行
        /// </summary>
        public VTextLine FirstLine { get; internal set; }

        /// <summary>
        /// 可视区域的最后一行
        /// </summary>
        public VTextLine LastLine { get; internal set; }

        /// <summary>
        /// 所属的文档
        /// </summary>
        public VTDocument OwnerDocument { get; internal set; }

        /// <summary>
        /// 总行数
        /// </summary>
        public int Rows { get { return this.options.Rows; } }

        /// <summary>
        /// 总列数
        /// </summary>
        public int Columns { get { return this.options.Columns; } }

        #endregion

        #region 构造方法

        public ViewableDocument(VTDocumentOptions options)
        {
            this.options = options;
        }

        #endregion

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

        /// <summary>
        /// 滚动可视区域
        /// 该函数不保证滚动到的区域有VTextLine，当滚动到的区域没有VTextLine的时候，那么有可能会崩溃...
        /// 所以在调用这个函数的时候，需要确保滚动到的区域有VTextLine
        /// </summary>
        /// <param name="orientation">滚动方向</param>
        /// <param name="scrollRows">要滚动的行数</param>
        /// <returns>可视区域第一行的指针</returns>
        public VTextLine ScrollDocument(ScrollOrientation orientation, int scrollRows)
        {
            VTextLine oldFirstLine = this.FirstLine;
            VTextLine oldLastLine = this.LastLine;
            VTextLine newFirstLine = null;
            VTextLine newLastLine = null;

            #region 更新新的可视区域的第一行和最后一行的指针

            switch (orientation)
            {
                case ScrollOrientation.Down:
                    {
                        // 往下滚动

                        newFirstLine = oldFirstLine.FindNext(scrollRows);
                        newLastLine = oldLastLine.FindNext(scrollRows);
                        break;
                    }

                case ScrollOrientation.Up:
                    {
                        newFirstLine = oldFirstLine.FindPrevious(scrollRows);
                        newLastLine = oldLastLine.FindPrevious(scrollRows);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.FirstLine = newFirstLine;
            this.LastLine = newLastLine;

            #endregion

            #region 计算从可视区域移出的行和移入的行

            // 从可视区域内被删除的行
            VTextLine removedFirst = null;
            // 新增加到可视区域内的行
            VTextLine addedFirst = null;

            if (scrollRows >= this.OwnerDocument.Rows)
            {
                // 此时说明已经移动了一整个屏幕了, 被复用的起始行和结束行就等于移动之前的起始行和结束行
                removedFirst = oldFirstLine;
                addedFirst = newFirstLine;
            }
            else
            {
                if (orientation == ScrollOrientation.Up)
                {
                    // 把可视区域往上移动
                    removedFirst = newLastLine.NextLine;
                    addedFirst = newFirstLine;
                }
                else if (orientation == ScrollOrientation.Down)
                {
                    // 把可视区域往下移动
                    removedFirst = oldFirstLine;
                    addedFirst = oldLastLine.NextLine;
                }
            }

            #endregion

            // 下次渲染的时候排版
            this.IsArrangeDirty = true;

            return newFirstLine;
        }

        /// <summary>
        /// 把所有的TextLine取消关联渲染模型
        /// </summary>
        public void DetachAll()
        {
            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                // 取消关联关系
                current.DetachDrawable();

                // 重置文本行
                current.DeleteAll();

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }
        }
    }
}
