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

        #region 解析Unicode字符用

        /// <summary>
        /// 当前是否正在解析Unicode字符
        /// </summary>
        public bool ParsingUnicode { get; set; }

        /// <summary>
        /// 当前剩余要解析的Unicode字符大小（字节为单位）
        /// </summary>
        public int UnicodeRemainSize { get; set; }

        /// <summary>
        /// 当前要解析的Unicode字符大小（字节为单位）
        /// </summary>
        public int UnicodeSize { get; set; }

        /// <summary>
        /// 存储Unicode字符的缓冲区
        /// </summary>
        public byte[] UnicodeBuff { get; set; }

        #endregion

        #region 当前数据流处理状态

        /// <summary>
        /// 当前终端收到的字符
        /// </summary>
        public byte Char { get; set; }

        /// <summary>
        /// 当前模式下，接收到的字符的控制功能
        /// </summary>
        public byte NextState;

        /// <summary>
        /// 当前接收到的字符与字符所对应的控制功能的映射关系表
        /// </summary>
        public byte[] StateTable = null;

        /// <summary>
        /// 当前控制模式
        /// </summary>
        public byte ControlFunction { get; set; }

        /// <summary>
        /// 存储CSI, OSC或DEC模式下的控制指令的参数
        /// </summary>
        public List<byte> ParameterBytes { get; private set; }

        #endregion

        /// <summary>
        /// 光标所在列
        /// </summary>
        public int CursorColumn { get; set; }

        /// <summary>
        /// 要在终端显示的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 把存储Unicode字节的缓冲区重置为0
        /// </summary>
        public void ResetUnicodeBuff()
        {
            for (int i = 0; i < 4; i++)
            {
                this.UnicodeBuff[i] = 0;
            }
        }
    }
}