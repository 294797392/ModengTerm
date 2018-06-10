using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalCore
{
    public class ParseState
    {
        public ParseState()
        {
            this.ParameterBytes = new List<byte>();
        }

        /// <summary>
        /// 当前模式下，接收到的字符的控制功能
        /// </summary>
        public byte NextState;

        /// <summary>
        /// 当前接收到的字符与字符所对应的控制功能的映射关系表
        /// </summary>
        public byte[] StateTable = null;

        /// <summary>
        /// 存储CSI或DEC模式下的控制指令的参数
        /// </summary>
        public List<byte> ParameterBytes { get; private set; }
    }
}