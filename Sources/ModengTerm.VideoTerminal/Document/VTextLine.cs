using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
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
    /// 存储当前Surface里显示的一行信息
    /// </summary>
    public class VTextLine : VTextElement
    {
        /// <summary>
        /// 指定要把行移动到的位置
        /// </summary>
        public enum MoveOptions
        {
            /// <summary>
            /// 移动到最后一行
            /// </summary>
            MoveToLast,

            /// <summary>
            /// 移动到第一行
            /// </summary>
            MoveToFirst
        }

        #region 实例变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTextLine");

        #endregion

        #region 实例变量

        /// <summary>
        /// 存储该行的字符列表
        /// </summary>
        private List<VTCharacter> characters;

        /// <summary>
        /// 已经显示了的列数
        /// 从1开始计数
        /// </summary>
        private int columns;

        private int physicsRow;

        #endregion

        #region 属性

        public override VTDocumentElements Type => VTDocumentElements.TextLine;

        /// <summary>
        /// 行索引，从0开始
        /// </summary>
        public int PhysicsRow
        {
            get { return this.physicsRow; }
            set
            {
                if (this.physicsRow != value)
                {
                    this.physicsRow = value;
#if DEBUG
                    // TODO：正式出版本的时候要删除这里，不然会影响运行速度
                    // 这里只是为了可以在渲染的时候看到最新的PhysicsRow，方便调试
                    this.SetRenderDirty(true);
#endif
                }
            }
        }

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
        /// 获取该行字符的只读集合
        /// </summary>
        public List<VTCharacter> Characters { get { return this.characters; } }

        #endregion

        #region 构造方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">该行所属的文档</param>
        public VTextLine(VTDocument owner) : base(owner)
        {
            this.characters = new List<VTCharacter>();
        }

        #endregion

        #region 实例方法

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

        /// <summary>
        /// 将该节点从VTDocument里移除
        /// </summary>
        internal void Remove()
        {
            VTextLine previous = this.PreviousLine;
            VTextLine next = this.NextLine;

            if (previous == null)
            {
                // 说明是第一行
                next.PreviousLine = null;
                this.OwnerDocument.FirstLine = next;
            }
            else if (next == null)
            {
                // 说明是最后一行
                previous.NextLine = null;
                this.OwnerDocument.LastLine = previous;
            }
            else
            {
                // 说明是中间的行
                previous.NextLine = next;
                next.PreviousLine = previous;
            }

            this.PreviousLine = null;
            this.NextLine = null;
        }

        /// <summary>
        /// 把一行挂载到该行后面
        /// </summary>
        /// <param name="textLine"></param>
        internal void Append(VTextLine textLine)
        {
            VTextLine next = this.NextLine;

            if (next != null)
            {
                // 该行的下一行不为空，说明该行不是最后一行
                next.PreviousLine = textLine;
            }
            else
            {
                // 如果该行的下一行是空，那么说明是最后一行
                // 此时要更新最后一行
                this.OwnerDocument.LastLine = textLine;
            }

            this.NextLine = textLine;
            textLine.PreviousLine = this;
            textLine.NextLine = next;
        }

        /// <summary>
        /// 把一行挂载到该行前面
        /// </summary>
        /// <param name="textLine"></param>
        internal void Prepend(VTextLine textLine)
        {
            // 分两种情况去处理：
            // 1. 该行是第一行，那么需要更新FirstLine指针
            // 2. 该行不是第一行

            if (this == this.OwnerDocument.FirstLine)
            {
                // 该行是第一行
                this.OwnerDocument.FirstLine = textLine;
                textLine.NextLine = this;
                this.PreviousLine = textLine;
            }
            else
            {
                // 该行不是第一行
                VTextLine previous = this.PreviousLine;
                previous.NextLine = textLine;
                textLine.PreviousLine = previous;
                textLine.NextLine = this;
                this.PreviousLine = textLine;
            }
        }

        private void EraseCharacter(int characterIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int index = characterIndex + i;
                VTCharacter character = this.characters[index];
                if (character.ColumnSize > 1)
                {
                    this.columns -= (character.ColumnSize - 1);
                }

                character.Character = ' ';
                character.ColumnSize = 1;
                character.Attribute = 0;
                character.Flags = VTCharacterFlags.None;
            }
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
        /// 该方法不会移动光标，单纯的设置某个列的字符
        /// </summary>
        /// <param name="character">要插入的字符</param>
        /// <param name="column">索引位置，在此处插入字符串</param>
        public void PrintCharacter(VTCharacter character, int column)
        {
            if (column + 1 > this.columns)
            {
                this.PadColumns(column + 1);
            }

            // 替换指定列的文本
            int characterIndex = this.FindCharacterIndex(column);
            if (characterIndex < 0)
            {
                logger.ErrorFormat("PrintCharacter失败, FindCharacterIndex失败, column = {0}", column);
                return;
            }

            VTCharacter oldCharacter = this.characters[characterIndex];

            this.characters.RemoveAt(characterIndex);
            this.columns -= oldCharacter.ColumnSize;

            this.characters.Insert(characterIndex, character);
            this.columns += character.ColumnSize;

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 从指定位置用空白符填充该行所有字符（包括指定位置处的字符）
        /// </summary>
        /// <param name="column">从哪一列开始填充</param>
        public void EraseToEnd(int column)
        {
            // TODO：所有被新加的字符都要沿用最后一个字符的Attribute?
            // htop第5行的背景不会显示到最后面

            int startIndex = this.FindCharacterIndex(column);
            if (startIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.FatalFormat("EraseToEnd失败, startIndex == -1");
                return;
            }

            int count = this.characters.Count - startIndex;

            this.EraseCharacter(startIndex, count);
        }

        /// <summary>
        /// 清除该行所有字符
        /// 做法是用空白符填充该行所有字符
        /// </summary>
        public void EraseAll()
        {
            this.EraseCharacter(0, this.characters.Count);
        }

        /// <summary>
        /// 删除从行首到光标处的内容
        /// </summary>
        /// <param name="column">光标位置</param>
        public void EraseFromBeginning(int column)
        {
            int characterIndex = this.FindCharacterIndex(column);
            if (characterIndex == -1)
            {
                // 按理说应该不会出现
                logger.FatalFormat("EraseFromBeginning失败, startIndex == -1, column = {0}", column);
                return;
            }

            this.EraseCharacter(0, characterIndex);
        }

        /// <summary>
        /// 从光标处用空格填充n个字符
        /// </summary>
        /// <param name="column">光标位置</param>
        /// <param name="characterCount">要填充的字符个数</param>
        public void EraseRange(int column, int characterCount)
        {
            int characterIndex = this.FindCharacterIndex(column);
            if (characterIndex == -1)
            {
                // 按理说应该不会出现
                logger.FatalFormat("EraseRange失败, startIndex == -1, column = {0}", column);
                return;
            }

            this.PadColumns(characterIndex + characterCount);
            this.EraseCharacter(characterIndex, characterCount);
        }

        /// <summary>
        /// 往前找到下一个VTextLine
        /// </summary>
        /// <param name="nrows">向前几行</param>
        /// <returns></returns>
        public VTextLine FindNext(int nrows)
        {
            VTextLine current = this;

            for (int i = 0; i < nrows; i++)
            {
                current = current.NextLine;

                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }

        /// <summary>
        /// 往后找到上一个VTextLine
        /// </summary>
        /// <param name="nrows">向后几行</param>
        /// <returns></returns>
        public VTextLine FindPrevious(int nrows)
        {
            VTextLine current = this;

            for (int i = 0; i < nrows; i++)
            {
                current = current.PreviousLine;

                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }

        /// <summary>
        /// 如果该行的列不足n个，那么补齐空字符直到有n列
        /// </summary>
        /// <param name="columns">要补齐到的列数。是从1开始的列数，而不是从0开始</param>
        public void PadColumns(int columns)
        {
            if (this.columns >= columns)
            {
                return;
            }

            // 要补齐的字符数
            int count = columns - this.columns;

            // 补齐字符
            for (int i = 0; i < count; i++)
            {
                this.characters.Add(VTCharacter.CreateNull());
            }

            this.columns += count;
        }

        /// <summary>
        /// 把一个历史行的数据应用到VTextLine上
        /// </summary>
        /// <param name="historyLine">要应用的历史行数据</param>
        public void SetHistory(VTHistoryLine historyLine)
        {
            VTUtils.CopyCharacter(historyLine.Characters, this.characters);
            this.PhysicsRow = historyLine.PhysicsRow;
            this.columns = VTUtils.GetColumns(this.characters);

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 从右边开始删除字符，直到该行字符小于等于指定的列数
        /// </summary>
        /// <param name="maxCol">该行最大列数</param>
        public void TrimEnd(int columns)
        {
            if (this.columns <= columns)
            {
                return;
            }

            // 要删除的列数
            int cols = this.columns - columns;
            while (cols > 0)
            {
                VTCharacter character = this.characters.LastOrDefault();
                if (character == null)
                {
                    break;
                }

                cols -= character.ColumnSize;
                this.columns -= character.ColumnSize;

                this.characters.Remove(character);
            }
        }

        /// <summary>
        /// 在指定字符索引处插入一个字符
        /// </summary>
        /// <param name="characterIndex">要插入字符的字符索引</param>
        /// <param name="character">要插入的字符</param>
        public void InsertCharacter(int characterIndex, VTCharacter character)
        {
            this.characters.Insert(characterIndex, character);
            this.columns += character.ColumnSize;
        }

        /// <summary>
        /// 删除指定列处的字符
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteCharacter(int column, int count)
        {
            if (column >= this.columns)
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

                // 更新显示的列数
                this.columns -= toDelete.ColumnSize;
            }

            this.SetRenderDirty(true);
        }

        #endregion
    }
}
