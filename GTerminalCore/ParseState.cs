using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GardeniaTerminalCore
{
    public class WideCharBuffer
    {

    }

    public class ParseState
    {
        public ParseState()
        {
            this.ParameterBytes = new List<byte>();
            this.EscDigits = new List<byte>();
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
        /// 当前处于的控制字符状态
        /// </summary>
        public byte ControlString { get; set; }

        /// <summary>
        /// 存储CSI, OSC或DEC模式下的控制指令的参数
        /// </summary>
        public List<byte> ParameterBytes { get; private set; }

        #endregion

        /// <summary>
        /// 在ESC状态下收到的数字参数
        /// digit in csi or dec mode
        /// </summary>
        public List<byte> EscDigits { get; private set; }

        /// <summary>
        /// 光标所在列
        /// </summary>
        public int CursorColumn { get; set; }

        /// <summary>
        /// 要在终端显示的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 重置为Ansi状态表
        /// 清空接收到的参数
        /// </summary>
        public void ResetState()
        {
            this.ParameterBytes.Clear();
            this.ControlString = 0;
            this.StateTable = VTPrsTbl.AnsiTable;
        }

        /// <summary>
        /// 清除Esc状态
        /// </summary>
        public void EscReset()
        {
            this.EscDigits.Clear();
            this.StateTable = VTPrsTbl.AnsiTable;
        }
    }
}