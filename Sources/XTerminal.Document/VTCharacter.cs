using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 存储终端里一个字符的信息
    /// VTCharacter保存要显示的字符信息和该字符的样式信息
    /// </summary>
    public class VTCharacter : VDocumentElement
    {
        /// <summary>
        /// 字符
        /// </summary>
        public char Character { get; set; }

        public VTCharacter(char character)
        {
            this.Character = character;
        }

        /// <summary>
        /// 把字符置空并且清除所有的文本样式
        /// </summary>
        public void Reset()
        {
            this.Character = ' ';
        }
    }
}
