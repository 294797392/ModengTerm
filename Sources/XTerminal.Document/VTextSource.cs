using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public enum VTextSources
    {
        StringTextSource,
        CharactersTextSource
    }

    public static class VTextSourceFactory
    {
        public static VTextSource Create(VTextSources type, int capacity)
        {
            switch (type)
            {
                case VTextSources.CharactersTextSource: return new VTCharactersTextSource(capacity);
                case VTextSources.StringTextSource: return new VTStringTextSource(capacity);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public abstract class VTextSource
    {
        protected static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(VTextSource));

        #region 实例变量

        protected int capacity;

        #endregion

        #region 属性

        /// <summary>
        /// 获取总列数
        /// </summary>
        public abstract int Columns { get; }

        /// <summary>
        /// 获取该文本数据源最大可以容纳多少列
        /// </summary>
        public int Capacity { get { return this.capacity; } }

        #endregion

        public VTextSource(int capacity)
        {
            this.capacity = capacity;
        }

        /// <summary>
        /// 重新设置该文本数据源最大的列数
        /// </summary>
        /// <param name="column"></param>
        public void ResizeColumn(int column)
        {
            if (column == this.capacity)
            {
                return;
            }

            if (column > this.capacity)
            {
                this.ResizeColumn(true, column - this.capacity);
            }
            else
            {
                this.ResizeColumn(false, this.capacity - column);
            }

            this.capacity = column;
        }

        #region 公开接口

        /// <summary>
        /// 通过在右侧填充指定的字符来达到指定的长度
        /// </summary>
        /// <param name="column"></param>
        /// <param name="padChar"></param>
        public void PadRight(int column, char padChar)
        {
            if (this.Columns >= column)
            {
                return;
            }

            int count = column - this.Columns;

            for (int i = 0; i < count; i++)
            {
                this.AddCharacter(padChar);
            }
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 设置该文本数据源的列数
        /// </summary>
        /// <param name="add">增加的列数</param>
        /// <param name="changedColumns">改变的列数</param>
        protected virtual void ResizeColumn(bool add, int changedColumns)
        { }

        /// <summary>
        /// 删除整行字符
        /// </summary>
        public abstract void DeleteAll();

        /// <summary>
        /// 获取文本数据
        /// </summary>
        /// <returns></returns>
        public abstract string GetText();

        public abstract void Insert(int column, char ch);
        public abstract void Remove(int column);
        public abstract void Remove(int column, int count);
        public abstract void SetCharacter(int column, char setChar);

        /// <summary>
        /// 在末尾Append一个字符
        /// </summary>
        /// <param name="addChar"></param>
        public abstract void AddCharacter(char addChar);

        #endregion
    }

    public class VTStringTextSource : VTextSource
    {
        private string text = string.Empty;

        public VTStringTextSource(int capacity) :
            base(capacity)
        {
        }

        public override int Columns => this.text.Length;

        public override void DeleteAll()
        {
            this.text = string.Empty;
        }

        public override string GetText()
        {
            return this.text;
        }

        public override void Insert(int column, char ch)
        {
            this.text = this.text.Insert(column, char.ToString(ch));
        }

        public override void AddCharacter(char addChar)
        {
            this.text = (this.text += char.ToString(addChar));
        }

        public override void Remove(int column)
        {
            this.text = this.text.Remove(column);
        }

        public override void Remove(int column, int count)
        {
            this.text = this.text.Remove(count, count);
        }

        public override void SetCharacter(int column, char setChar)
        {
            this.text = this.text.Remove(column, 1).Insert(column, char.ToString(setChar));
        }
    }

    public class VTCharactersTextSource : VTextSource
    {
        private List<VTCharacter> characters;

        public override int Columns => this.characters.Count;

        public VTCharactersTextSource(int capacity) :
            base(capacity)
        {
            this.characters = new List<VTCharacter>();
        }

        #region 实例方法

        private VTCharacter CreateCharacter(char ch)
        {
            return new VTCharacter(ch);
        }

        #endregion

        #region 重写方法

        public override void DeleteAll()
        {
            this.characters.Clear();
        }

        public override string GetText()
        {
            string text = string.Empty;

            foreach (VTCharacter character in this.characters)
            {
                text += character.Character;
            }

            return text;
        }

        public override void Insert(int column, char ch)
        {
            this.characters.Insert(column, this.CreateCharacter(ch));
        }

        public override void Remove(int column)
        {
            this.characters.RemoveRange(column, this.characters.Count - column);
        }

        public override void Remove(int column, int count)
        {
            this.characters.RemoveRange(column, count);
        }

        public override void AddCharacter(char addChar)
        {
            this.characters.Add(this.CreateCharacter(addChar));
        }

        public override void SetCharacter(int column, char setChar)
        {
            this.characters[column].Character = setChar;
        }

        #endregion
    }
}
