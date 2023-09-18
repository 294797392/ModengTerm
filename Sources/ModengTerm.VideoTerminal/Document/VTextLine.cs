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

        private int[] decorationVersions;

        /// <summary>
        /// 已经显示了的列数
        /// </summary>
        private int columns;

        #endregion

        #region 属性

        public override VTDocumentElements Type => VTDocumentElements.TextLine;

        /// <summary>
        /// 行索引，从0开始
        /// </summary>
        public int PhysicsRow { get; set; }

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
            this.decorationVersions = new int[Enum.GetValues(typeof(VTextAttributes)).Length];
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

            // 算出来要打印的列与当前行的最后一列之间相差多少列
            int value = column + 1 - this.columns;

            // 只相差一列，说明是在该行最后一个字符的后面打印字符，可以直接添加字符
            if (value == 1)
            {
                this.characters.Add(character);
                this.columns += character.ColumnSize;
            }
            else if (value > 1)
            {
                // 相差了多列，那么创建空字符填充相差的列，然后在指定位置打印要打印的字符
                int count = column - this.columns;

                for (int i = 0; i < count - 1; i++)
                {
                    VTCharacter nullCharacter = VTCharacter.CreateNull();
                    this.characters.Add(nullCharacter);
                    this.columns += nullCharacter.ColumnSize;
                }
                this.characters.Add(character);
                this.columns += character.ColumnSize;
            }
            else
            {
                // 不相差列，说明要在已有列中替换字符
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
            }

            //if (column == this.OwnerDocument.ColumnSize - 1)
            //{
            //    //logger.ErrorFormat("光标在最右边");
            //    // 此时说明光标在最右边
            //    this.CursorAtRightMargin = true;
            //}

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
            if (column >= this.columns)
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

                // 更新显示的列数
                this.columns -= toDelete.ColumnSize;
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

        /// <summary>
        /// 1. 删除整行字符
        /// </summary>
        public void DeleteAll()
        {
            this.characters.Clear();
            this.columns = 0;

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 从指定的位置开始使用空白字符串填充剩下的所有字符（包括指定位置处的字符）
        /// </summary>
        /// <param name="column"></param>
        public void Erase(int column)
        {
            if (column + 1 > this.columns)
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

                this.columns -= character.ColumnSize - 1;

                // TODO：这里是否需要回收字符以便于节省内存占用？
                character.Character = ' ';
                character.ColumnSize = 1;
                character.AttributeList.Clear();
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
        /// <param name="nrows">向前几行</param>
        /// <returns></returns>
        public VTextLine FindNext(int nrows)
        {
            VTextLine current = this;

            for (int i = 0; i < nrows; i++)
            {
                current = current.NextLine;
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
            this.PrintCharacter(VTCharacter.CreateNull(), columns - 1);
        }

        /// <summary>
        /// 把一个历史行的数据应用到VTextLine上
        /// </summary>
        /// <param name="historyLine">要应用的历史行数据</param>
        public void SetHistory(VTHistoryLine historyLine)
        {
            this.characters.Clear();
            this.characters.AddRange(historyLine.Characters);
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

        public VTextData BuildData()
        {
            VTextData textData = new VTextData();

            for (int i = 0; i < this.Characters.Count; i++)
            {
                VTCharacter character = this.Characters[i];

                textData.Text += character.Character;

                foreach (VTextAttributeState attributeState in character.AttributeList)
                {
                    VTextAttribute attribute = textData.Attributes.FirstOrDefault(v => v.Attribute == attributeState.Attribute && !v.Closed);

                    if (attributeState.Enabled)
                    {
                        // 启用状态
                        if (attribute == null)
                        {
                            attribute = new VTextAttribute()
                            {
                                Attribute = attributeState.Attribute,
                                StartIndex = i,
                                Parameter = attributeState.Parameter
                            };
                            textData.Attributes.Add(attribute);
                        }
                        else
                        {
                            // 颜色比较特殊，有可能连续多次设置不同的颜色
                            if (attributeState.Attribute == VTextAttributes.Background ||
                                attributeState.Attribute == VTextAttributes.Foreground)
                            {
                                // 如果设置的是颜色的话，并且当前字符的颜色和最后一次设置的颜色不一样，那么要先关闭最后一次设置的颜色
                                // attribute是最后一次设置的颜色，attributeState是当前字符的颜色
                                if (attribute.Parameter != attributeState.Parameter)
                                {
                                    attribute.Closed = true;

                                    // 关闭后创建一个新的Attribute
                                    attribute = new VTextAttribute()
                                    {
                                        Attribute = attributeState.Attribute,
                                        StartIndex = i,
                                        Parameter = attributeState.Parameter
                                    };
                                    textData.Attributes.Add(attribute);
                                }
                            }
                        }
                        attribute.Count++;
                    }
                    else
                    {
                        // 禁用状态
                        if (attribute != null)
                        {
                            attribute.Closed = true;
                        }
                    }
                }
            }

            return textData;
        }

        #endregion
    }
}
