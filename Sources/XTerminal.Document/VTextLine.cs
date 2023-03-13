using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XTerminal.Document
{
    /// <summary>
    /// 1. 对文本行进行排版，分块
    /// 2. 维护行的测量信息
    /// </summary>
    public class VTextLine : VDocumentElement
    {
        #region 实例变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTextLine");

        #endregion

        #region 实例变量

        private bool isCharacterDirty;

        #endregion

        #region 属性

        /// <summary>
        /// 终端行的最大列数
        /// 规定终端一行里的字符数不能超过列数
        /// 超过列数要按照手册里定义的标准来执行动作
        /// 在linux里使用stty size获取
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// 该行所有的文本块
        /// </summary>
        private List<VTextBlock> TextBlocks { get; set; }

        /// <summary>
        /// 上一个文本行
        /// </summary>
        public VTextLine PreviousLine { get; set; }

        /// <summary>
        /// 下一个文本行
        /// </summary>
        public VTextLine NextLine { get; set; }

        ///// <summary>
        ///// 第一个文本块
        ///// </summary>
        //public VTextBlock FirstBlock { get; private set; }

        ///// <summary>
        ///// 最后一个文本块
        ///// </summary>
        //public VTextBlock LastBlock { get; private set; }

        /// <summary>
        /// 画图对象
        /// </summary>
        public IDrawingElement DrawingElement { get; set; }

        /// <summary>
        /// 是否开启了DECAWM模式
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 当前光标是否在最右边
        /// </summary>
        public bool CursorAtRightMargin { get; set; }

        /// <summary>
        /// 所属的文档
        /// </summary>
        public VTDocument OwnerDocument { get; set; }
        
        /// <summary>
        /// 文本数据源
        /// </summary>
        public VTextSource TextSource { get; private set; }

        ///// <summary>
        ///// 该行的所有字符
        ///// </summary>
        //private List<VTCharacter> Characters { get; set; }

        /// <summary>
        /// 表示该行是否有字符被修改了，需要重新渲染
        /// </summary>
        public bool IsCharacterDirty
        {
            get
            {
                return this.isCharacterDirty;
            }
            set
            {
                this.isCharacterDirty = value;
            }
        }

        #endregion

        #region 构造方法

        public VTextLine(int initialColumns)
        {
            this.Columns = initialColumns;
            //this.Characters = new List<VTCharacter>();
            this.TextBlocks = new List<VTextBlock>();
            this.TextSource = VTextSourceFactory.Create(VTextSources.CharactersTextSource, initialColumns);
            this.SetDirty();
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 把该行设置为脏行
        /// 表示下次会重绘
        /// </summary>
        private void SetDirty()
        {
            if (!this.IsCharacterDirty)
            {
                this.IsCharacterDirty = true;
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 设置指定位置处的字符
        /// </summary>
        /// <param name="ch">要插入的字符</param>
        /// <param name="column">索引位置，在此处插入字符串</param>
        public void PrintCharacter(char ch, int column)
        {
            //if (this.CursorAtRightMargin && this.DECPrivateAutoWrapMode)
            //{
            //    // 说明光标已经在最右边了
            //    // 并且开启了自动换行(DECAWM)的功能，那么要自动换行

            //    // 换行完了之后再重置状态
            //    this.CursorAtRightMargin = false;
            //}
            //else
            //{
            // 更新文本
            this.TextSource.PrintCharacter(ch, column);

            this.SetDirty();

            //}

            ////if (this.Columns == this)
            //{
            //    // 光标在最右边了，下次在收到字符，要自动换行了
            //    // 在收到移动光标指令的时候，要清除这个标志
            //    this.CursorAtRightMargin = true;
            //}
        }

        /// <summary>
        /// 从指定位置开始删除字符串
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        public void DeleteText(int column)
        {
            this.TextSource.DeleteText(column);

            this.SetDirty();
        }

        /// <summary>
        /// 删除指定位置处的字符串
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteText(int column, int count)
        {
            this.TextSource.DeleteText(column, count);

            this.SetDirty();
        }

        /// <summary>
        /// 删除整行字符
        /// </summary>
        public void DeleteAll()
        {
            this.TextSource.DeleteAll();

            this.SetDirty();
        }

        public string BuildText()
        {
            return this.TextSource.GetText();
        }

        /// <summary>
        /// 往前找到下一个VTextLine
        /// </summary>
        /// <param name="rows">向前几行</param>
        /// <returns></returns>
        public VTextLine FindNext(int rows)
        {
            VTextLine current = this;

            for (int i = 0; i < rows; i++)
            {
                current = current.NextLine;
            }

            return current;
        }

        /// <summary>
        /// 往后找到上一个VTextLine
        /// </summary>
        /// <param name="rows">向后几行</param>
        /// <returns></returns>
        public VTextLine FindPrevious(int rows)
        {
            VTextLine current = this;

            for (int i = 0; i < rows; i++)
            {
                current = current.PreviousLine;
            }

            return current;
        }

        #endregion
    }
}
