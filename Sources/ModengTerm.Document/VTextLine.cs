using ModengTerm.Document.Drawing;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ModengTerm.Document
{
    /// <summary>
    /// 文本行对象
    /// </summary>
    public class VTextLine : VTElement
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

        public override DrawingObjectTypes Type => DrawingObjectTypes.TextLine;

        /// <summary>
        /// 行索引，从0开始
        /// </summary>
        public int PhysicsRow
        {
            get { return physicsRow; }
            set
            {
                if (physicsRow != value)
                {
                    physicsRow = value;
#if DEBUG
                    // TODO：正式出版本的时候要删除这里，不然会影响运行速度
                    // 这里只是为了可以在渲染的时候看到最新的PhysicsRow，方便调试
                    SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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
        /// 获取该行字符的只读集合
        /// </summary>
        public List<VTCharacter> Characters { get { return characters; } }

        /// <summary>
        /// 获取该行一共占用了多少列
        /// 一个中文字符占用2列，其他的占用1列
        /// </summary>
        public int Columns { get { return this.columns; } }

        /// <summary>
        /// 文本的测量信息
        /// </summary>
        public VTextMetrics Metrics { get; private set; }

        /// <summary>
        /// 获取该文本块的宽度
        /// 包含空白字符
        /// </summary>
        public double Width { get { return Metrics.Width; } }

        /// <summary>
        /// 该行高度，当DECAWM被设置的时候，终端里的一行如果超出了列数，那么会自动换行
        /// 当一行的字符超过终端的列数的时候，DECAWM指令指定了超出的字符要如何处理
        /// DECAWM SET：超出后要在新的一行上从头开始显示字符
        /// DECAWM RESET：超出后在该行的第一个字符处开始显示字符
        /// </summary>
        public double Height { get { return this.Typeface.Height; return Metrics.Height; } }

        /// <summary>
        /// 获取该文本的边界框信息
        /// 在画完之后会更新测量的矩形框信息
        /// </summary>
        public VTRect Bounds { get { return new VTRect(OffsetX, OffsetY, Width, Height); } }

        /// <summary>
        /// 文本样式
        /// </summary>
        public VTypeface Typeface { get; set; }

        #endregion

        #region 构造方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">该行所属的文档</param>
        public VTextLine(VTDocument owner) :
            base(owner)
        {
            characters = new List<VTCharacter>();
            Metrics = new VTextMetrics();
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
            int characterIndex = FindCharacterIndex(column);
            if (characterIndex == -1)
            {
                return null;
            }

            return characters[characterIndex];
        }

        /// <summary>
        /// 将该节点从VTDocument里移除
        /// </summary>
        internal void Remove()
        {
            VTextLine previous = PreviousLine;
            VTextLine next = NextLine;

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

            PreviousLine = null;
            NextLine = null;
        }

        /// <summary>
        /// 把一行挂载到该行后面
        /// </summary>
        /// <param name="textLine"></param>
        internal void Append(VTextLine textLine)
        {
            VTextLine next = NextLine;

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

            NextLine = textLine;
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
                PreviousLine = textLine;
            }
            else
            {
                // 该行不是第一行
                VTextLine previous = PreviousLine;
                previous.NextLine = textLine;
                textLine.PreviousLine = previous;
                textLine.NextLine = this;
                PreviousLine = textLine;
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

            foreach (VTCharacter character in characters)
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
            if (column + 1 > columns)
            {
                PadColumns(column + 1);
            }

            // 替换指定列的文本
            int characterIndex = FindCharacterIndex(column);
            if (characterIndex < 0)
            {
                logger.ErrorFormat("PrintCharacter失败, FindCharacterIndex失败, column = {0}", column);
                return;
            }

            VTCharacter oldCharacter = characters[characterIndex];

            characters.RemoveAt(characterIndex);
            columns -= oldCharacter.ColumnSize;

            characters.Insert(characterIndex, character);
            columns += character.ColumnSize;

            SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 从指定位置用空白符填充该行所有字符（包括指定位置处的字符）
        /// </summary>
        /// <param name="column">从哪一列开始填充</param>
        public void EraseToEnd(int column)
        {
            int startIndex = FindCharacterIndex(column);
            if (startIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.FatalFormat("EraseToEnd失败, startIndex == -1");
                return;
            }

            int count = characters.Count - startIndex;

            EraseRange(startIndex, count);
        }

        /// <summary>
        /// 用空白字符填充该行所有字符
        /// </summary>
        public void EraseAll()
        {
            EraseRange(0, characters.Count);
        }

        /// <summary>
        /// 使用空白字符填充从行首到指定列处的内容
        /// 如果指定列不存在则什么都不做
        /// </summary>
        /// <param name="column">光标位置</param>
        public void EraseFromBeginning(int column)
        {
            int characterIndex = FindCharacterIndex(column);
            if (characterIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.WarnFormat("EraseFromBeginning失败, startIndex == -1, column = {0}", column);
                return;
            }

            EraseRange(0, characterIndex + 1);
        }

        /// <summary>
        /// 用空白字符填充指定区域内的字符
        /// </summary>
        /// <param name="column">要从第几列开始擦除字符</param>
        /// <param name="count">要擦除的字符个数</param>
        public void EraseRange(int column, int count)
        {
            if (count == 0)
            {
                return;
            }

            if (column >= columns)
            {
                logger.WarnFormat("EraseRange失败，删除的索引位置在字符之外");
                return;
            }

            int startIndex = FindCharacterIndex(column);
            if (startIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.WarnFormat("DeleteText失败, startIndex == -1, column = {0}, count = {1}", column, count);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                VTCharacter character = characters[startIndex + i];
                if (character.ColumnSize > 1)
                {
                    columns -= character.ColumnSize - 1;
                }

                character.Character = ' ';
                character.ColumnSize = 1;
                character.Attribute = 0;
                character.Flags = VTCharacterFlags.None;
            }

            SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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
                characters.Add(VTCharacter.CreateNull());
            }

            this.columns += count;

            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 把一个历史行的数据应用到VTextLine上
        /// </summary>
        /// <param name="historyLine">要应用的历史行数据</param>
        public void SetHistory(VTHistoryLine historyLine)
        {
            VTUtils.CopyCharacter(historyLine.Characters, characters);
            PhysicsRow = historyLine.PhysicsRow;
            columns = VTUtils.GetColumns(characters);

            SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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
                VTCharacter character = characters.LastOrDefault();
                if (character == null)
                {
                    break;
                }

                cols -= character.ColumnSize;
                this.columns -= character.ColumnSize;

                characters.Remove(character);
            }

            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 在指定字符索引处插入一个字符
        /// </summary>
        /// <param name="characterIndex">要插入字符的字符索引</param>
        /// <param name="character">要插入的字符</param>
        public void InsertCharacter(int characterIndex, VTCharacter character)
        {
            characters.Insert(characterIndex, character);
            columns += character.ColumnSize;
            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 从指定列开始删除字符，删到结尾
        /// </summary>
        /// <param name="column">要从第几列开始删除</param>
        public void Delete(int column) 
        {
            int startIndex = FindCharacterIndex(column);
            if (startIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.WarnFormat("Delete失败, startIndex == -1, column = {0}", column);
                return;
            }

            int count = this.characters.Count - startIndex;

            for (int i = 0; i < count; i++)
            {
                VTCharacter toDelete = characters[startIndex];
                characters.Remove(toDelete);

                // 更新总列数
                columns -= toDelete.ColumnSize;
            }

            SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 删除指定列处的字符
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteRange(int column, int count)
        {
            if (count == 0)
            {
                return;
            }

            if (column >= columns)
            {
                logger.WarnFormat("DeleteText失败，删除的索引位置在字符之外");
                return;
            }

            int startIndex = FindCharacterIndex(column);
            if (startIndex == -1)
            {
                // 按理说应该不会出现，因为在上面已经判断过列是否超出范围了
                logger.FatalFormat("DeleteText失败, startIndex == -1, column = {0}, count = {1}", column, count);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                VTCharacter toDelete = characters[startIndex];
                characters.Remove(toDelete);

                // 更新总列数
                columns -= toDelete.ColumnSize;
            }

            SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 删除该行所有字符
        /// </summary>
        public void DeleteAll()
        {
            this.DeleteRange(0, this.characters.Count);
        }

        #endregion

        #region VTElement

        protected override void OnInitialize(IDrawingObject drawingObject)
        {
        }

        protected override void OnRelease()
        {
        }

        protected override void OnRender()
        {
            IDrawingTextLine drawingTextLine = this.DrawingObject as IDrawingTextLine;

            VTFormattedText formattedText = VTUtils.CreateFormattedText(characters);
            formattedText.Style = this.Typeface;

            // 把物理行号打印出来，调试用
            //formattedText.Text = string.Format("{0} - {1}", this.PhysicsRow, formattedText.Text);

            drawingTextLine.FormattedText = formattedText;

            drawingTextLine.Draw();

            VTextMetrics textMetrics = drawingTextLine.Measure();
            Metrics = textMetrics;
        }

        /// <summary>
        /// 测量该行里指定的子文本矩形框
        /// </summary>
        /// <param name="startIndex">要测量的起始字符索引</param>
        /// <param name="count">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        public VTextRange MeasureTextRange(int startIndex, int count)
        {
            IDrawingTextLine objectText = DrawingObject as IDrawingTextLine;

            return objectText.MeasureTextRange(startIndex, count);
        }

        /// <summary>
        /// 测量一行里某个字符的测量信息
        /// 注意该接口只能测量出来X偏移量，Y偏移量需要外部根据高度自己计算
        /// </summary>
        /// <param name="characterIndex">要测量的字符</param>
        /// <returns>文本坐标，X=文本左边的X偏移量，Y永远是0，因为边界框是相对于该行的</returns>
        public VTextRange MeasureCharacter(int characterIndex)
        {
            return this.MeasureTextRange(characterIndex, 1);
        }

        #endregion
    }
}
