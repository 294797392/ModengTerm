using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminalParser;

namespace XTerminal.Drawing
{
    public enum DeleteCharacterFrom
    {
        /// <summary>
        /// 从前往后删除字符
        /// </summary>
        FrontToBack,

        /// <summary>
        /// 从后往前删除字符
        /// </summary>
        BackToFront
    }

    public class VTextBlock : VTextElement<VTextBlock>
    {
        /// <summary>
        /// TextBlock的索引号
        /// </summary>
        public string ID { get; set; }

        ///// <summary>
        ///// 要显示的文本
        ///// </summary>
        //public string Text { get; set; }

        /// <summary>
        /// 字体格式
        /// </summary>
        public VTextStyle Style { get; set; }

        /// <summary>
        /// 该文本块第一个字符所在列数，从0开始
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 获取该文本块所占据的列数
        /// </summary>
        public int Columns { get; internal set; }

        /// <summary>
        /// 所属行
        /// </summary>
        public VTextLine OwnerLine { get; internal set; }

        /// <summary>
        /// 与WPF关联的画图对象
        /// </summary>
        public object DrawingObject { get; set; }

        public VTextBlock()
        {
        }

        ///// <summary>
        ///// 向文本块里插入字符
        ///// </summary>
        ///// <param name="text"></param>
        //public void InsertText(char text)
        //{
        //    this.Text += text;
        //}

        //public void InsertText(string text)
        //{
        //    this.Text += text;
        //}

        ///// <summary>
        ///// 删除字符
        ///// </summary>
        ///// <param name="startIndex">要删除的字符的起始位置</param>
        ///// <param name="count">要删除的字符的个数</param>
        //public void DeleteText(int startIndex, int count)
        //{
        //    if (this.Text.Length == 0)
        //    {
        //        return;
        //    }

        //    this.Text = this.Text.Remove(startIndex, count);
        //}

        ///// <summary>
        ///// 删除从startIndex到结尾的所有字符
        ///// </summary>
        ///// <param name="startIndex"></param>
        //public void DeleteText(int startIndex)
        //{
        //    if (this.Text.Length == 0)
        //    {
        //        return;
        //    }

        //    this.Text = this.Text.Remove(startIndex);
        //}

        ///// <summary>
        ///// 删除字符
        ///// </summary>
        ///// <param name="from">指定删除字符的位置</param>
        ///// <param name="count">指定要删除的字符个数</param>
        //public void DeleteText(DeleteCharacterFrom from, int count)
        //{
        //    switch (from)
        //    {
        //        case DeleteCharacterFrom.FrontToBack:
        //            {
        //                this.DeleteText(0, count);
        //                break;
        //            }

        //        case DeleteCharacterFrom.BackToFront:
        //            {
        //                int startIndex = this.Text.Length - count;
        //                this.DeleteText(startIndex, count);
        //                break;
        //            }

        //        default:
        //            throw new NotImplementedException();
        //    }
        //}

        /// <summary>
        /// 该文本块是否有内容
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return this.Columns == 0;
        }
    }
}
