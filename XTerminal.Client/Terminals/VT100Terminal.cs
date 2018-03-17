using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XTerminal.Terminal;
using XTerminalCore;

namespace XTerminal.Terminals
{
    /// <summary>
    /// VT100类型的终端模拟器
    /// VT100中的ControlSequenceIntroducer字符是ESC[
    /// </summary>
    public class VT100Terminal : AbstractTerminal
    {
        /// <summary>
        /// 设置或获取当按下Retuen键时，所发送的数据
        /// 对于VT100来说，Enter键就是Return键
        /// VT100有两种选项，可以选择发送CR(\r)也可以发送CRLF(\r\n)
        /// 默认值是CRLF
        /// </summary>
        public byte[] ReturnKeyCode
        {
            get
            {
                byte[] result;
                NonalphabeticKeyCode.TryGetValue(Keys.Enter, out result);
                return result;
            }
            set
            {
                NonalphabeticKeyCode[Keys.Enter] = value;
            }
        }

        /// <summary>
        /// 设置或获取文本的编码方式
        /// </summary>
        public Encoding Encoding { get; set; }

        public VT100Terminal()
        {
            this.Encoding = Encoding.ASCII;
        }

        #region 非字符ascii码转换

        /*
            把按住shift键和没按住shift键的非字母字符转换成VT100识别的sdcii码
            参考:
                xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
                xterminal/Dependencies/VT100 User Guide/table3-3.html
        */

        private static Dictionary<Keys, byte[]> ShiftNonalphabeticKeyCode = new Dictionary<Keys, byte[]>()
        {
            { Keys.Enter, new byte[] { AnsiChars.CR } },
            { Keys.Space, new byte[] { AnsiChars.Space } },

            { Keys.Backspace, new byte[] { AnsiChars.BS } },
            { Keys.Tab, new byte[] { AnsiChars.Tab } },

            { Keys.CusorUp, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.A } },
            { Keys.CursorDown, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.B } },
            { Keys.CursorLeft, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.D } },
            { Keys.CursorRight, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.C } },

            { Keys.D1, new byte[]{ Convert.ToByte("041", 8) } },
            { Keys.D2, new byte[]{ Convert.ToByte("100", 8) } },
            { Keys.D3, new byte[]{ Convert.ToByte("043", 8) } },
            { Keys.D4, new byte[]{ Convert.ToByte("044", 8) } },
            { Keys.D5, new byte[]{ Convert.ToByte("045", 8) } },
            { Keys.D6, new byte[]{ Convert.ToByte("136", 8) } },
            { Keys.D7, new byte[]{ Convert.ToByte("046", 8) } },
            { Keys.D8, new byte[]{ Convert.ToByte("052", 8) } },
            { Keys.D9, new byte[]{ Convert.ToByte("050", 8) } },
            { Keys.D0, new byte[]{ Convert.ToByte("051", 8) } },
            { Keys.OemMinus, new byte[]{ Convert.ToByte("137", 8) } },
            { Keys.OemPlus, new byte[]{ Convert.ToByte("053", 8) } },
            { Keys.OemOpenBrackets, new byte[]{ Convert.ToByte("173", 8) } },
            { Keys.Oem1, new byte[]{ Convert.ToByte("072", 8) } },
            { Keys.OemQuotes, new byte[]{ Convert.ToByte("042", 8) } },
            { Keys.OemComma, new byte[]{ Convert.ToByte("074", 8) } },
            { Keys.OemPeriod, new byte[]{ Convert.ToByte("076", 8) } },
            { Keys.OemQuestion, new byte[]{ Convert.ToByte("077", 8) } },
            { Keys.Oem5, new byte[]{ Convert.ToByte("174", 8) } },
            { Keys.Oem3, new byte[]{ Convert.ToByte("176", 8) } },
            { Keys.Oem6, new byte[]{ Convert.ToByte("175", 8) } },
        };
        private static Dictionary<Keys, byte[]> NonalphabeticKeyCode = new Dictionary<Keys, byte[]>()
        {
            { Keys.Enter, new byte[] { AnsiChars.CR } },
            { Keys.Space, new byte[] { AnsiChars.Space } },

            { Keys.Backspace, new byte[] { AnsiChars.BS } },
            { Keys.Tab, new byte[] { AnsiChars.Tab } },

            { Keys.CusorUp, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.A } },
            { Keys.CursorDown, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.B } },
            { Keys.CursorLeft, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.D } },
            { Keys.CursorRight, new byte[] { ControlFunctions.ESC, Fe.CSI_7BIT, AnsiChars.C } },

            { Keys.D1, new byte[]{ Convert.ToByte("061", 8) } },
            { Keys.D2, new byte[]{ Convert.ToByte("062", 8) } },
            { Keys.D3, new byte[]{ Convert.ToByte("063", 8) } },
            { Keys.D4, new byte[]{ Convert.ToByte("064", 8) } },
            { Keys.D5, new byte[]{ Convert.ToByte("065", 8) } },
            { Keys.D6, new byte[]{ Convert.ToByte("066", 8) } },
            { Keys.D7, new byte[]{ Convert.ToByte("067", 8) } },
            { Keys.D8, new byte[]{ Convert.ToByte("070", 8) } },
            { Keys.D9, new byte[]{ Convert.ToByte("071", 8) } },
            { Keys.D0, new byte[]{ Convert.ToByte("060", 8) } },
            { Keys.OemMinus, new byte[]{ Convert.ToByte("055", 8) } },
            { Keys.OemPlus, new byte[]{ Convert.ToByte("075", 8) } },
            { Keys.OemOpenBrackets,  new byte[] { Convert.ToByte("133", 8) } },
            { Keys.Oem1, new byte[] { Convert.ToByte("073", 8) } },
            { Keys.OemQuotes, new byte[] { Convert.ToByte("047", 8)} },
            { Keys.OemComma, new byte[] { Convert.ToByte("054", 8) } },
            { Keys.OemPeriod, new byte[] { Convert.ToByte("056", 8) } },
            { Keys.OemQuestion, new byte[] { Convert.ToByte("057", 8)} },
            { Keys.Oem5, new byte[] { Convert.ToByte("134", 8) } },
            { Keys.Oem3, new byte[] { Convert.ToByte("140", 8) } },
            { Keys.Oem6, new byte[] { Convert.ToByte("135", 8) } },
        };

        #endregion

        #region 把按住Control+Key的组合键转换成VT100的命令字符

        /// <summary>
        /// 把按住Control+Key的组合键转换成VT100的命令字符
        /// 参考：
        ///     xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
        ///     xterminal/Dependencies/VT100 User Guide/table3-5.html
        /// </summary>
        private static Dictionary<Keys, byte[]> ControlKeyCode = new Dictionary<Keys, byte[]>()
        {
            { Keys.A, new byte[] { Convert.ToByte("001", 8) } },// SOH
            { Keys.B, new byte[] { Convert.ToByte("002", 8) } },// STX
            { Keys.C, new byte[] { Convert.ToByte("003", 8) } },// ETX
            { Keys.D, new byte[] { Convert.ToByte("004", 8) } },// EOT
            { Keys.E, new byte[] { Convert.ToByte("005", 8) } },// ENQ
            { Keys.F, new byte[] { Convert.ToByte("006", 8) } },// ACK
            { Keys.G, new byte[] { Convert.ToByte("007", 8) } },// BELL
            { Keys.H, new byte[] { Convert.ToByte("010", 8) } },// BS
            { Keys.I, new byte[] { Convert.ToByte("011", 8) } },// HT
            { Keys.J, new byte[] { Convert.ToByte("012", 8) } },// LF
            { Keys.K, new byte[] { Convert.ToByte("013", 8) } },// VT
            { Keys.L, new byte[] { Convert.ToByte("014", 8) } },// FF
            { Keys.M, new byte[] { Convert.ToByte("015", 8) } },// CR
            { Keys.N, new byte[] { Convert.ToByte("016", 8) } },// SO
            { Keys.O, new byte[] { Convert.ToByte("017", 8) } },// SI
            { Keys.P, new byte[] { Convert.ToByte("020", 8) } },// DLE
            { Keys.Q, new byte[] { Convert.ToByte("021", 8) } },// DC1 or XON
            { Keys.R, new byte[] { Convert.ToByte("022", 8) } },// DC2
            { Keys.S, new byte[] { Convert.ToByte("023", 8) } },// DC3 or XOFF
            { Keys.T, new byte[] { Convert.ToByte("024", 8) } },// DC4
            { Keys.U, new byte[] { Convert.ToByte("025", 8) } },// NAK
            { Keys.V, new byte[] { Convert.ToByte("026", 8) } },// SYN
            { Keys.W, new byte[] { Convert.ToByte("027", 8) } },// ETB
            { Keys.X, new byte[] { Convert.ToByte("030", 8) } },// CAN
            { Keys.Y, new byte[] { Convert.ToByte("031", 8) } },// EM
            { Keys.Z, new byte[] { Convert.ToByte("032", 8) } },// SUB

            { Keys.Space, new byte[] { Convert.ToByte("000", 8) } },// Space Bar NUL
            { Keys.OemOpenBrackets, new byte[] { Convert.ToByte("033", 8) } }, //[ ESC
            { Keys.Oem5, new byte[] { Convert.ToByte("034", 8) } },// \ FS
            { Keys.Oem6, new byte[] { Convert.ToByte("035", 8) } },// ] GS
            { Keys.Oem3, new byte[] { Convert.ToByte("036", 8) } },// ~ RS
            { Keys.OemQuestion, new byte[] { Convert.ToByte("037", 8) } },// ? US
        };

        #endregion

        #region 大小写字母字符转ascii码

        /// <summary>
        /// 把大小写字母转成VT100识别的ASCII码
        /// 参考:
        ///     xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
        ///     xterminal/Dependencies/VT100 User Guide/table3-2.html
        /// </summary>
        private static Dictionary<Keys, byte[]> UpperCaseKeyCode = new Dictionary<Keys, byte[]>()
        {
            { Keys.A, new byte[] { Convert.ToByte("101", 8) } },// SOH
            { Keys.B, new byte[] { Convert.ToByte("102", 8) } },// STX
            { Keys.C, new byte[] { Convert.ToByte("103", 8) } },// ETX
            { Keys.D, new byte[] { Convert.ToByte("104", 8) } },// EOT
            { Keys.E, new byte[] { Convert.ToByte("105", 8) } },// ENQ
            { Keys.F, new byte[] { Convert.ToByte("106", 8) } },// ACK
            { Keys.G, new byte[] { Convert.ToByte("107", 8) } },// BELL
            { Keys.H, new byte[] { Convert.ToByte("110", 8) } },// BS
            { Keys.I, new byte[] { Convert.ToByte("111", 8) } },// HT
            { Keys.J, new byte[] { Convert.ToByte("112", 8) } },// LF
            { Keys.K, new byte[] { Convert.ToByte("113", 8) } },// VT
            { Keys.L, new byte[] { Convert.ToByte("114", 8) } },// FF
            { Keys.M, new byte[] { Convert.ToByte("115", 8) } },// CR
            { Keys.N, new byte[] { Convert.ToByte("116", 8) } },// SO
            { Keys.O, new byte[] { Convert.ToByte("117", 8) } },// SI
            { Keys.P, new byte[] { Convert.ToByte("120", 8) } },// DLE
            { Keys.Q, new byte[] { Convert.ToByte("121", 8) } },// DC1 or XON
            { Keys.R, new byte[] { Convert.ToByte("122", 8) } },// DC2
            { Keys.S, new byte[] { Convert.ToByte("123", 8) } },// DC3 or XOFF
            { Keys.T, new byte[] { Convert.ToByte("124", 8) } },// DC4
            { Keys.U, new byte[] { Convert.ToByte("125", 8) } },// NAK
            { Keys.V, new byte[] { Convert.ToByte("126", 8) } },// SYN
            { Keys.W, new byte[] { Convert.ToByte("127", 8) } },// ETB
            { Keys.X, new byte[] { Convert.ToByte("130", 8) } },// CAN
            { Keys.Y, new byte[] { Convert.ToByte("131", 8) } },// EM
            { Keys.Z, new byte[] { Convert.ToByte("132", 8) } },// SUB
        };
        private static Dictionary<Keys, byte[]> LowerCaseKeyCode = new Dictionary<Keys, byte[]>()
        {
            { Keys.A, new byte[] { Convert.ToByte("141", 8) } },// SOH
            { Keys.B, new byte[] { Convert.ToByte("142", 8) } },// STX
            { Keys.C, new byte[] { Convert.ToByte("143", 8) } },// ETX
            { Keys.D, new byte[] { Convert.ToByte("144", 8) } },// EOT
            { Keys.E, new byte[] { Convert.ToByte("145", 8) } },// ENQ
            { Keys.F, new byte[] { Convert.ToByte("146", 8) } },// ACK
            { Keys.G, new byte[] { Convert.ToByte("147", 8) } },// BELL
            { Keys.H, new byte[] { Convert.ToByte("150", 8) } },// BS
            { Keys.I, new byte[] { Convert.ToByte("151", 8) } },// HT
            { Keys.J, new byte[] { Convert.ToByte("152", 8) } },// LF
            { Keys.K, new byte[] { Convert.ToByte("153", 8) } },// VT
            { Keys.L, new byte[] { Convert.ToByte("154", 8) } },// FF
            { Keys.M, new byte[] { Convert.ToByte("155", 8) } },// CR
            { Keys.N, new byte[] { Convert.ToByte("156", 8) } },// SO
            { Keys.O, new byte[] { Convert.ToByte("157", 8) } },// SI
            { Keys.P, new byte[] { Convert.ToByte("160", 8) } },// DLE
            { Keys.Q, new byte[] { Convert.ToByte("161", 8) } },// DC1 or XON
            { Keys.R, new byte[] { Convert.ToByte("162", 8) } },// DC2
            { Keys.S, new byte[] { Convert.ToByte("163", 8) } },// DC3 or XOFF
            { Keys.T, new byte[] { Convert.ToByte("164", 8) } },// DC4
            { Keys.U, new byte[] { Convert.ToByte("165", 8) } },// NAK
            { Keys.V, new byte[] { Convert.ToByte("166", 8) } },// SYN
            { Keys.W, new byte[] { Convert.ToByte("167", 8) } },// ETB
            { Keys.X, new byte[] { Convert.ToByte("170", 8) } },// CAN
            { Keys.Y, new byte[] { Convert.ToByte("171", 8) } },// EM
            { Keys.Z, new byte[] { Convert.ToByte("172", 8) } },// SUB
        };

        #endregion

        protected override byte[] TranslateKey(PressedKey key)
        {
            byte[] translatedByte = { };

            // 检测非字母Shift键按下
            if (key.IsShiftPressed)
            {
                if (ShiftNonalphabeticKeyCode.TryGetValue(key.Key, out translatedByte))
                {
                    return translatedByte;
                }
            }
            // 检测非字母+Shift键没按下
            else
            {
                if (NonalphabeticKeyCode.TryGetValue(key.Key, out translatedByte))
                {
                    return translatedByte;
                }
            }

            // 检测Control键按下
            if (key.IsControlPressed)
            {
                if (ControlKeyCode.TryGetValue(key.Key, out translatedByte))
                {
                    return translatedByte;
                }
            }

            // 检测大写字母
            if (key.IsUpperCase)
            {
                if (UpperCaseKeyCode.TryGetValue(key.Key, out translatedByte))
                {
                    return translatedByte;
                }
            }
            // 检测小写字母
            else
            {
                if (LowerCaseKeyCode.TryGetValue(key.Key, out translatedByte))
                {
                    return translatedByte;
                }
            }

            return translatedByte;
        }

        protected override void ProcessReceivedData(byte[] data)
        {
        }
    }
}