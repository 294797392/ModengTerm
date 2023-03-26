using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 1. 对文本行进行排版，分块
    /// 2. 维护行的测量信息
    /// </summary>
    public class VTextLine : VTextElement
    {
        #region 实例变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTextLine");

        #endregion

        #region 实例变量

        /// <summary>
        /// 存储该行的字符列表
        /// </summary>
        private List<VTCharacter> characters;

        #endregion

        #region 属性

        public int ID { get; set; }

        public override VTDocumentElements Type => VTDocumentElements.TextLine;

        /// <summary>
        /// 列大小
        /// 规定终端一行里的字符数不能超过列数
        /// 超过列数要按照手册里定义的标准来执行动作
        /// 在linux里使用stty size获取
        /// </summary>
        public int ColumnSize { get; set; }

        /// <summary>
        /// 已经显示了的列数
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// 上一个文本行
        /// </summary>
        public VTextLine PreviousLine { get; set; }

        /// <summary>
        /// 下一个文本行
        /// </summary>
        public VTextLine NextLine { get; set; }

        /// <summary>
        /// 是否开启了DECAWM模式
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 当前光标是否在最右边
        /// </summary>
        public bool CursorAtRightMargin { get; private set; }

        /// <summary>
        /// 获取该行的文本，如果字符数量是0，那么返回一个空白字符，目的是可以测量出来文本的测量信息
        /// </summary>
        public string Text
        {
            get
            {
                string text = this.GetText();
                return text.Length == 0 ? " " : text;
            }
        }

        /// <summary>
        /// 获取该行字符的只读集合
        /// </summary>
        public IEnumerable<VTCharacter> Characters { get { return this.characters; } }

        #endregion

        #region 构造方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">该行所属的文档</param>
        public VTextLine(VTDocument owner) : base(owner)
        {
            this.ColumnSize = owner.ColumnSize;
            this.characters = new List<VTCharacter>();
        }

        #endregion

        #region 实例方法

        private string GetText()
        {
            string text = string.Empty;

            foreach (VTCharacter character in this.characters)
            {
                text += character.Character;
            }

            return text;
        }

        /// <summary>
        /// 查找某列的字符
        /// 注意一个字符可能占两列
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private VTCharacter FindCharacter(int column)
        {
            int characterIndex = this.FindCharacterIndex(column);
            if (characterIndex == -1)
            {
                return null;
            }

            return this.characters[characterIndex];
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 查找某列的字符在集合中的索引
        /// 注意一个字符可能占两列
        /// </summary>
        /// <param name="column"></param>
        /// <returns>找不到返回-1</returns>
        public int FindCharacterIndex(int column)
        {
            int index = 0;

            int startColumn = 0;
            int endColumn = 0;

            foreach (VTCharacter character in this.characters)
            {
                endColumn = startColumn + character.ColumnSize;

                if (column >= startColumn && column < endColumn)
                {
                    return index;
                }

                startColumn += character.ColumnSize;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// 设置指定位置处的字符
        /// </summary>
        /// <param name="character">要插入的字符</param>
        /// <param name="column">索引位置，在此处插入字符串</param>
        public void PrintCharacter(VTCharacter character, int column)
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

            int value = column + 1 - this.Columns;

            if (value == 1)
            {
                // 说明是在该行最后一个字符的后面打印字符
                this.characters.Add(character);
                this.Columns += character.ColumnSize;
            }
            else if (value > 1)
            {
                int count = column - this.Columns;

                for (int i = 0; i < count - 1; i++)
                {
                    VTCharacter nullCharacter = VTCharacter.CreateNull();
                    this.characters.Add(nullCharacter);
                    this.Columns += nullCharacter.ColumnSize;
                }
                this.characters.Add(character);
                this.Columns += character.ColumnSize;
            }
            else
            {
                // 替换指定列的文本
                VTCharacter oldCharacte = this.FindCharacter(column);
                if (oldCharacte == null)
                {
                    logger.ErrorFormat("PrintCharacter失败, FindCharacter失败, column = {0}", column);
                    return;
                }

                int delta = character.ColumnSize - oldCharacte.ColumnSize;
                this.Columns += delta;

                oldCharacte.Character = character.Character;
                oldCharacte.ColumnSize = character.ColumnSize;
                oldCharacte.Flags = character.Flags;
            }

            if (column == this.ColumnSize - 1)
            {
                //logger.ErrorFormat("光标在最右边");
                // 此时说明光标在最右边
                this.CursorAtRightMargin = true;
            }

            this.SetRenderDirty(true);
            //}

            //if (this.Columns == this)
            //{
            //    // 光标在最右边了，下次在收到字符，要自动换行了
            //    // 在收到移动光标指令的时候，要清除这个标志
            //    this.CursorAtRightMargin = true;
            //}
        }

        /// <summary>
        /// 删除指定列的字符
        /// </summary>
        /// <param name="column">要删除的列</param>
        public void DeleteText(int column)
        {
            if (column >= this.Columns)
            {
                logger.WarnFormat("DeleteText失败，删除的索引位置在字符之外");
                return;
            }

            int startIndex = this.FindCharacterIndex(column);
            if (startIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.FatalFormat("DeleteText失败, startIndex == -1, column = {0}", column);
                return;
            }

            int remain = this.characters.Count - startIndex;

            for (int i = 0; i < remain; i++)
            {
                VTCharacter toDelete = this.characters[startIndex];
                this.characters.Remove(toDelete);
                VTCharacter.Recycle(toDelete);

                // 更新显示的列数
                this.Columns -= toDelete.ColumnSize;
            }

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 删除指定列处的字符
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteText(int column, int count)
        {
            if (column >= this.Columns)
            {
                logger.WarnFormat("DeleteText失败，删除的索引位置在字符之外");
                return;
            }

            int startIndex = this.FindCharacterIndex(column);
            if (startIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.FatalFormat("DeleteText失败, startIndex == -1, column = {0}, count = {1}", column, count);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                VTCharacter toDelete = this.characters[startIndex];
                this.characters.Remove(toDelete);
                VTCharacter.Recycle(toDelete);

                // 更新显示的列数
                this.Columns -= toDelete.ColumnSize;
            }

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 删除整行字符
        /// </summary>
        public void DeleteAll()
        {
            foreach (VTCharacter character in this.characters)
            {
                VTCharacter.Recycle(character);
            }

            this.characters.Clear();
            this.Columns = 0;

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 从指定的位置开始使用空白字符串填充剩下的所有字符（包括指定位置处的字符）
        /// </summary>
        /// <param name="column"></param>
        public void Erase(int column)
        {
            if (column + 1 > this.Columns)
            {
                return;
            }

            int startIndex = this.FindCharacterIndex(column);
            if (startIndex == -1)
            {
                logger.ErrorFormat("ReplaceToEnd失败，startIndex == -1, column = {0}", column);
                return;
            }

            for (int i = startIndex; i < this.characters.Count; i++)
            {
                VTCharacter character = this.characters[i];

                this.Columns -= character.ColumnSize - 1;

                character.Character = ' ';
                character.ColumnSize = 1;
                character.Flags = VTCharacterFlags.SingleByteChar;
            }

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 用空白符填充该行所有字符
        /// </summary>
        public void EraseAll()
        {
            this.Erase(0);
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

        /// <summary>
        /// 把一个历史行的数据应用到VTextLine上
        /// </summary>
        /// <param name="historyLine">要应用的历史行数据</param>
        public void SetHistory(VTHistoryLine historyLine)
        {
            this.characters.Clear();
            this.characters.AddRange(historyLine.Characters);

            this.SetRenderDirty(true);
        }

        #endregion
    }
}
