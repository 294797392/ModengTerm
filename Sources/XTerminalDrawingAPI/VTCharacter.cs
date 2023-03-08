using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 存储终端里一个字符的信息
    /// </summary>
    public class VTCharacter : VTextElement
    {
        /// <summary>
        /// 字符
        /// </summary>
        public char Character { get; set; }

        public VTCharacter(char character)
        {
            this.Character = character;
        }
    }
}
