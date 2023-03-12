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
        public static VTextSource Create(VTextSources type, int initialColumns)
        {
            switch (type)
            {
                case VTextSources.CharactersTextSource: return new VTCharactersTextSource(initialColumns);
                case VTextSources.StringTextSource: return new VTStringTextSource(initialColumns);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public abstract class VTextSource
    {
        protected int columns;

        public VTextSource(int column)
        {
            this.columns = column;
        }

        /// <summary>
        /// 重新设置该文本数据源最大的列数
        /// </summary>
        /// <param name="column"></param>
        public void ResizeColumn(int column)
        {
            if (column == this.columns)
            {
                return;
            }

            if (column > this.columns)
            {
                this.ResizeColumn(true, column - this.columns);
            }
            else
            {
                this.ResizeColumn(false, this.columns - column);
            }

            this.columns = column;
        }

        /// <summary>
        /// 设置该文本数据源的列数
        /// </summary>
        /// <param name="add">增加的列数</param>
        /// <param name="changedColumns">改变的列数</param>
        protected abstract void ResizeColumn(bool add, int changedColumns);

        /// <summary>
        /// 在指定的列打印字符
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="column"></param>
        public abstract void PrintCharacter(char ch, int column);

        /// <summary>
        /// 从指定位置开始删除字符串
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        public abstract void DeleteText(int column);

        /// <summary>
        /// 删除指定位置处的字符串
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        /// <param name="count">要删除的字符个数</param>
        public abstract void DeleteText(int column, int count);

        /// <summary>
        /// 删除整行字符
        /// </summary>
        public abstract void DeleteAll();

        /// <summary>
        /// 获取文本数据
        /// </summary>
        /// <returns></returns>
        public abstract string GetText();
    }

    public class VTStringTextSource : VTextSource
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTStringTextSource");

        private string text = string.Empty;

        public VTStringTextSource(int column) :
            base(column)
        {
            this.text = string.Empty.PadRight(this.columns, ' ');
        }

        public override void DeleteAll()
        {
            this.text = string.Empty.PadRight(this.columns, ' ');
        }

        public override void DeleteText(int column)
        {
            if (column >= this.text.Length)
            {
                logger.WarnFormat("DeleteText，删除的索引位置在字符之外");
                return;
            }

            this.text = this.text.Remove(column).PadRight(this.columns, ' ');
        }

        public override void DeleteText(int column, int count)
        {
            if (column >= this.text.Length)
            {
                logger.WarnFormat("DeleteText，删除的索引位置在字符之外");
                return;
            }

            this.text = this.text.Remove(columns, count);
            this.text = this.text.Insert(columns, string.Empty.PadRight(count, ' '));
        }

        public override string GetText()
        {
            return this.text;
        }

        public override void PrintCharacter(char ch, int column)
        {
            this.text.Remove(column, 1).Insert(column, ch.ToString());
        }

        protected override void ResizeColumn(bool add, int changedColumns)
        {
            if (add)
            {
                this.text = this.text.PadRight(this.columns, ' ');
            }
            else
            {
                this.text = this.text.Substring(0, this.text.Length - changedColumns);
            }
        }
    }

    public class VTCharactersTextSource : VTextSource
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTCharactersTextSource");

        private List<VTCharacter> characters;

        public VTCharactersTextSource(int column) :
            base(column)
        {
            this.characters = new List<VTCharacter>();
            for (int i = 0; i < column; i++)
            {
                VTCharacter character = new VTCharacter(' ');
                this.characters.Add(character);
            }
        }

        public override void DeleteAll()
        {
            this.characters.ForEach((character) => { character.Character = ' '; });
        }

        public override void DeleteText(int column)
        {
            if (column >= this.characters.Count)
            {
                logger.WarnFormat("DeleteText，删除的索引位置在字符之外");
                return;
            }

            int deletes = this.characters.Count - column;

            for (int i = 0; i < deletes; i++)
            {
                this.characters[column + i].Character = ' ';
            }
        }

        public override void DeleteText(int column, int count)
        {
            if (column >= this.characters.Count)
            {
                logger.WarnFormat("DeleteText，删除的索引位置在字符之外");
                return;
            }

            // 最多能删几个字符
            int maxDeletes = this.characters.Count - column;

            int deletes = Math.Min(maxDeletes, count);

            for (int i = 0; i < deletes; i++)
            {
                this.characters[column + i].Character = ' ';
            }
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

        public override void PrintCharacter(char ch, int column)
        {
            if (column + 1 > this.characters.Count)
            {
                int count = (column + 1) - this.characters.Count;
                for (int i = 0; i < count; i++)
                {
                    VTCharacter character = new VTCharacter(' ');
                    this.characters.Add(character);
                }
                this.characters[column].Character = ch;
            }
            else
            {
                this.characters[column].Character = ch;
            }
        }

        protected override void ResizeColumn(bool add, int changedColumns)
        {
            if (add)
            {
                for (int i = 0; i < changedColumns; i++)
                {
                    VTCharacter character = new VTCharacter(' ');
                    this.characters.Add(character);
                }
            }
            else
            {
                for (int i = 0; i < changedColumns; i++)
                {
                    this.characters.RemoveAt(this.characters.Count - 1);
                }
            }
        }
    }
}
