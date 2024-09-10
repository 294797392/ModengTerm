using ModengTerm.Document.Drawing;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml;

namespace ModengTerm.Document
{
    /// <summary>
    /// 文本行对象
    /// </summary>
    public class VTextLine : VTElement
    {
        #region 实例变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTextLine");

        #endregion

        #region 实例变量

        /// <summary>
        /// 已经显示了的列数
        /// 从1开始计数
        /// </summary>
        private int columns;

        private int physicsRow;

        private VTFormattedText formattedText;

        #endregion

        #region 属性

        public override DocumentObjectTypes Type => DocumentObjectTypes.TextLine;

        /// <summary>
        /// 物理行号，从0开始
        /// TODO：考虑删掉，如果在编辑文档的时候，删除了一行，那么所有的HistoryLine都要更新，这会是一个灾难
        /// 解决方法是在VTDocument里保存当前显示的第一行的索引
        /// </summary>
        //        public int PhysicsRow
        //        {
        //            get { return physicsRow; }
        //            set
        //            {
        //                if (physicsRow != value)
        //                {
        //                    physicsRow = value;
        //#if DEBUG
        //                    // TODO：正式出版本的时候要删除这里，不然会影响运行速度
        //                    // 这里只是为了可以在渲染的时候看到最新的PhysicsRow，方便调试
        //                    SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        //#endif
        //                }
        //            }
        //        }

        /// <summary>
        /// 该行对应的历史记录
        /// 每一个VTextLine都和一个VTHistoryLine对应
        /// 当修改Characters的时候，直接对History里的Characters进行修改，这样就不需要每次都去手动更新VTHistoryLine了
        /// VTHistoryLine保存显示该行所需要的最基本的数据，不包含运行时数据
        /// </summary>
        public VTHistoryLine History { get; private set; }

        /// <summary>
        /// 上一个文本行
        /// </summary>
        public VTextLine PreviousLine { get; set; }

        /// <summary>
        /// 下一个文本行
        /// </summary>
        public VTextLine NextLine { get; set; }

        /// <summary>
        /// 获取该行字符的集合
        /// </summary>
        public List<VTCharacter> Characters { get { return this.History.Characters; } }

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
            this.Metrics = new VTextMetrics();
            this.History = new VTHistoryLine();
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

            return this.Characters[characterIndex];
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

            foreach (VTCharacter character in this.Characters)
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
        /// 根据字符索引查找该字符所在的列索引
        /// </summary>
        /// <param name="characterIndex"></param>
        /// <returns></returns>
        public int FindCharacterColumn(int characterIndex)
        {
            int column = 0;

            for (int i = 0; i < this.Characters.Count; i++)
            {
                VTCharacter character = this.Characters[i];

                if (i == characterIndex)
                {
                    return column;
                }

                column += character.ColumnSize;
            }

            return column;
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

            VTCharacter oldCharacter = this.Characters[characterIndex];

            this.Characters.RemoveAt(characterIndex);
            columns -= oldCharacter.ColumnSize;

            this.Characters.Insert(characterIndex, character);
            columns += character.ColumnSize;

            SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 从指定位置用空白符填充该行所有字符（包括指定位置处的字符）
        /// </summary>
        /// <param name="column">从哪一列开始填充, 该行也会被擦除</param>
        public void EraseToEnd(int column)
        {
            int characterIndex = FindCharacterIndex(column);
            if (characterIndex == -1)
            {
                logger.WarnFormat("EraseToEnd失败, 没有找到对应列的字符, column = {0}", column);
                return;
            }

            // 先把该列和后面的所有字符删除
            int characters = this.Characters.Count - characterIndex;
            for (int i = 0; i < characters; i++)
            {
                VTCharacter toRemove = this.Characters[characterIndex];

                this.Characters.RemoveAt(characterIndex);

                this.columns -= toRemove.ColumnSize;
            }

            // 再新建空字符

            // 总列数
            int totalCols = this.OwnerDocument.ViewportColumn;

            // 要擦除的列数
            int eraseCols = totalCols - this.columns;

            for (int i = 0; i < eraseCols; i++)
            {
                VTCharacter character = VTCharacter.CreateNull();

                this.Characters.Add(character);
            }

            this.columns = totalCols;

            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        /// <summary>
        /// 用空白字符填充该行所有字符
        /// </summary>
        public void EraseAll()
        {
            this.Characters.Clear();

            int totalColumns = this.OwnerDocument.ViewportColumn;

            for (int i = 0; i < totalColumns; i++)
            {
                VTCharacter character = VTCharacter.CreateNull();

                this.Characters.Add(character);
            }

            this.columns = totalColumns;

            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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
                VTCharacter character = this.Characters[startIndex + i];
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
                this.Characters.Add(VTCharacter.CreateNull());
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
            this.History = historyLine;
            this.columns = VTUtils.GetColumns(this.Characters);
            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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
                VTCharacter character = this.Characters.LastOrDefault();
                if (character == null)
                {
                    break;
                }

                cols -= character.ColumnSize;
                this.columns -= character.ColumnSize;

                this.Characters.Remove(character);
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
            this.Characters.Insert(characterIndex, character);
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
                //logger.WarnFormat("Delete失败, startIndex == -1, column = {0}", column);
                return;
            }

            int count = this.Characters.Count - startIndex;

            for (int i = 0; i < count; i++)
            {
                VTCharacter toDelete = this.Characters[startIndex];
                this.Characters.Remove(toDelete);

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
                VTCharacter toDelete = this.Characters[startIndex];
                this.Characters.Remove(toDelete);

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
            this.DeleteRange(0, this.Characters.Count);
        }

        /// <summary>
        /// 获取该行的物理行号
        /// </summary>
        /// <returns></returns>
        public int GetPhysicsRow()
        {
            // 先找到逻辑行号

            int logicalRow = 0;

            VTDocument document = this.OwnerDocument;
            VTScrollInfo scrollInfo = document.Scrollbar;

            VTextLine textLine = document.FirstLine;

            while (textLine != null)
            {
                if (textLine == this) 
                {
                    break;
                    //return scrollInfo.ScrollValue + logicalRow;
                }

                logicalRow++;

                textLine = textLine.NextLine;
            }

            if (scrollInfo.Maximum == 0)
            {
                return logicalRow;
            }
            else
            {
                return scrollInfo.Value + logicalRow;
            }
        }

        #endregion

        #region VTElement

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
            this.History = null;
        }

        protected override void OnRender()
        {
            VTFormattedText formattedText = VTUtils.CreateFormattedText(this.Characters);
            formattedText.Style = this.Typeface;

            this.formattedText = formattedText;

            // 把物理行号打印出来，调试用
            //formattedText.Text = string.Format("{0} - {1}", this.PhysicsRow, formattedText.Text);

            this.Metrics = this.DrawingObject.DrawText(formattedText);
        }

        /// <summary>
        /// 测量该行里指定的子文本矩形框
        /// 测量出来的Y偏移量就是该文本相对于文档左上角的偏移量
        /// </summary>
        /// <param name="startIndex">要测量的起始字符索引</param>
        /// <param name="count">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        public VTextRange MeasureTextRange(int startIndex, int count)
        {
            return this.DrawingObject.MeasureText(this.formattedText, startIndex, count);
        }

        /// <summary>
        /// 测量一行里某个字符的测量信息
        /// 测量出来的Y偏移量就是该文本相对于文档左上角的偏移量
        /// </summary>
        /// <param name="characterIndex">要测量的字符</param>
        /// <returns></returns>
        public VTextRange MeasureCharacter(int characterIndex)
        {
            return this.MeasureTextRange(characterIndex, 1);
        }

        #endregion
    }
}
