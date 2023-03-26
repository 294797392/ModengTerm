using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public enum VTCharacterFlags
    {
        /// <summary>
        /// 单字节字符，占用1列
        /// </summary>
        SingleByteChar,

        /// <summary>
        /// 多字节字符
        /// 通过字符编码方式去决定这个char占用几列
        /// </summary>
        MulitByteChar
    }
}
