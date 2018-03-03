using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XTerminal.Terminal;

namespace XTerminal.Terminals
{
    /// <summary>
    /// VT100类型的终端模拟器
    /// VT100中的ControlSequenceIntroducer字符是ESC[
    /// </summary>
    public class VT100Terminal : AbstractTerminal
    {
        #region 非字符ascii码转换

        /*
            把按住shift键和没按住shift键的非字母字符转换成VT100识别的sdcii码
            参考:
                xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
                xterminal/Dependencies/VT100 User Guide/table3-3.html
        */

        private static Dictionary<Keys, byte> ShiftNonalphabeticKeyCode = new Dictionary<Keys, byte>()
        {
            { Keys.D1, Convert.ToByte("061", 8) },
            { Keys.D2, Convert.ToByte("062", 8) },
            { Keys.D3, Convert.ToByte("063", 8) },
            { Keys.D4, Convert.ToByte("064", 8) },
            { Keys.D5, Convert.ToByte("065", 8) },
            { Keys.D6, Convert.ToByte("066", 8) },
            { Keys.D7, Convert.ToByte("067", 8) },
            { Keys.D8, Convert.ToByte("070", 8) },
            { Keys.D9, Convert.ToByte("071", 8) },
            { Keys.D0, Convert.ToByte("060", 8) },
            { Keys.OemMinus, Convert.ToByte("055", 8) },
            { Keys.OemPlus, Convert.ToByte("075", 8) },
            { Keys.OemOpenBrackets, Convert.ToByte("133", 8) },
            { Keys.Oem1, Convert.ToByte("073", 8) },
            { Keys.OemQuotes, Convert.ToByte("047", 8) },
            { Keys.OemComma, Convert.ToByte("054", 8) },
            { Keys.OemPeriod, Convert.ToByte("056", 8) },
            { Keys.OemQuestion, Convert.ToByte("057", 8) },
            { Keys.Oem5, Convert.ToByte("134", 8) },
            { Keys.Oem3, Convert.ToByte("140", 8) },
            { Keys.Oem6, Convert.ToByte("135", 8) },
        };
        private static Dictionary<Keys, byte> NonalphabeticKeyCode = new Dictionary<Keys, byte>()
        {
            { Keys.D1, Convert.ToByte("041", 8) },
            { Keys.D2, Convert.ToByte("100", 8) },
            { Keys.D3, Convert.ToByte("043", 8) },
            { Keys.D4, Convert.ToByte("044", 8) },
            { Keys.D5, Convert.ToByte("045", 8) },
            { Keys.D6, Convert.ToByte("136", 8) },
            { Keys.D7, Convert.ToByte("046", 8) },
            { Keys.D8, Convert.ToByte("052", 8) },
            { Keys.D9, Convert.ToByte("050", 8) },
            { Keys.D0, Convert.ToByte("051", 8) },
            { Keys.OemMinus, Convert.ToByte("137", 8) },
            { Keys.OemPlus, Convert.ToByte("053", 8) },
            { Keys.OemOpenBrackets, Convert.ToByte("173", 8) },
            { Keys.Oem1, Convert.ToByte("072", 8) },
            { Keys.OemQuotes, Convert.ToByte("042", 8) },
            { Keys.OemComma, Convert.ToByte("074", 8) },
            { Keys.OemPeriod, Convert.ToByte("076", 8) },
            { Keys.OemQuestion, Convert.ToByte("077", 8) },
            { Keys.Oem5, Convert.ToByte("174", 8) },
            { Keys.Oem3, Convert.ToByte("176", 8) },
            { Keys.Oem6, Convert.ToByte("175", 8) },
        };

        #endregion

        #region 把按住Control+Key的组合键转换成VT100的命令字符

        /// <summary>
        /// 把按住Control+Key的组合键转换成VT100的命令字符
        /// 参考：
        ///     xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
        ///     xterminal/Dependencies/VT100 User Guide/table3-5.html
        /// </summary>
        private static Dictionary<Keys, byte> ControlKeyCode = new Dictionary<Keys, byte>()
        {
            { Keys.A, Convert.ToByte("001", 8) },// SOH
            { Keys.B, Convert.ToByte("002", 8) },// STX
            { Keys.C, Convert.ToByte("003", 8) },// ETX
            { Keys.D, Convert.ToByte("004", 8) },// EOT
            { Keys.E, Convert.ToByte("005", 8) },// ENQ
            { Keys.F, Convert.ToByte("006", 8) },// ACK
            { Keys.G, Convert.ToByte("007", 8) },// BELL
            { Keys.H, Convert.ToByte("010", 8) },// BS
            { Keys.I, Convert.ToByte("011", 8) },// HT
            { Keys.J, Convert.ToByte("012", 8) },// LF
            { Keys.K, Convert.ToByte("013", 8) },// VT
            { Keys.L, Convert.ToByte("014", 8) },// FF
            { Keys.M, Convert.ToByte("015", 8) },// CR
            { Keys.N, Convert.ToByte("016", 8) },// SO
            { Keys.O, Convert.ToByte("017", 8) },// SI
            { Keys.P, Convert.ToByte("020", 8) },// DLE
            { Keys.Q, Convert.ToByte("021", 8) },// DC1 or XON
            { Keys.R, Convert.ToByte("022", 8) },// DC2
            { Keys.S, Convert.ToByte("023", 8) },// DC3 or XOFF
            { Keys.T, Convert.ToByte("024", 8) },// DC4
            { Keys.U, Convert.ToByte("025", 8) },// NAK
            { Keys.V, Convert.ToByte("026", 8) },// SYN
            { Keys.W, Convert.ToByte("027", 8) },// ETB
            { Keys.X, Convert.ToByte("030", 8) },// CAN
            { Keys.Y, Convert.ToByte("031", 8) },// EM
            { Keys.Z, Convert.ToByte("032", 8) },// SUB

            { Keys.Space, Convert.ToByte("000", 8) },// Space Bar NUL
            { Keys.OemOpenBrackets, Convert.ToByte("033", 8) }, //[ ESC
            { Keys.Oem5, Convert.ToByte("034", 8) },// \ FS
            { Keys.Oem6, Convert.ToByte("035", 8) },// ] GS
            { Keys.Oem3, Convert.ToByte("036", 8) },// ~ RS
            { Keys.OemQuestion, Convert.ToByte("037", 8) },// ? US
        };

        #endregion

        #region 大小写字母字符转ascii码

        /// <summary>
        /// 把大小写字母转成VT100识别的ASCII码
        /// 参考:
        ///     xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
        ///     xterminal/Dependencies/VT100 User Guide/table3-2.html
        /// </summary>

        private static Dictionary<Keys, byte> UpperCaseKeyCode = new Dictionary<Keys, byte>()
        {
            { Keys.A, Convert.ToByte("101", 8) },// SOH
            { Keys.B, Convert.ToByte("102", 8) },// STX
            { Keys.C, Convert.ToByte("103", 8) },// ETX
            { Keys.D, Convert.ToByte("104", 8) },// EOT
            { Keys.E, Convert.ToByte("105", 8) },// ENQ
            { Keys.F, Convert.ToByte("106", 8) },// ACK
            { Keys.G, Convert.ToByte("107", 8) },// BELL
            { Keys.H, Convert.ToByte("110", 8) },// BS
            { Keys.I, Convert.ToByte("111", 8) },// HT
            { Keys.J, Convert.ToByte("112", 8) },// LF
            { Keys.K, Convert.ToByte("113", 8) },// VT
            { Keys.L, Convert.ToByte("114", 8) },// FF
            { Keys.M, Convert.ToByte("115", 8) },// CR
            { Keys.N, Convert.ToByte("116", 8) },// SO
            { Keys.O, Convert.ToByte("117", 8) },// SI
            { Keys.P, Convert.ToByte("120", 8) },// DLE
            { Keys.Q, Convert.ToByte("121", 8) },// DC1 or XON
            { Keys.R, Convert.ToByte("122", 8) },// DC2
            { Keys.S, Convert.ToByte("123", 8) },// DC3 or XOFF
            { Keys.T, Convert.ToByte("124", 8) },// DC4
            { Keys.U, Convert.ToByte("125", 8) },// NAK
            { Keys.V, Convert.ToByte("126", 8) },// SYN
            { Keys.W, Convert.ToByte("127", 8) },// ETB
            { Keys.X, Convert.ToByte("130", 8) },// CAN
            { Keys.Y, Convert.ToByte("131", 8) },// EM
            { Keys.Z, Convert.ToByte("132", 8) },// SUB
        };
        private static Dictionary<Keys, byte> LowerCaseKeyCode = new Dictionary<Keys, byte>()
        {
            { Keys.A, Convert.ToByte("141", 8) },// SOH
            { Keys.B, Convert.ToByte("142", 8) },// STX
            { Keys.C, Convert.ToByte("143", 8) },// ETX
            { Keys.D, Convert.ToByte("144", 8) },// EOT
            { Keys.E, Convert.ToByte("145", 8) },// ENQ
            { Keys.F, Convert.ToByte("146", 8) },// ACK
            { Keys.G, Convert.ToByte("147", 8) },// BELL
            { Keys.H, Convert.ToByte("150", 8) },// BS
            { Keys.I, Convert.ToByte("151", 8) },// HT
            { Keys.J, Convert.ToByte("152", 8) },// LF
            { Keys.K, Convert.ToByte("153", 8) },// VT
            { Keys.L, Convert.ToByte("154", 8) },// FF
            { Keys.M, Convert.ToByte("155", 8) },// CR
            { Keys.N, Convert.ToByte("156", 8) },// SO
            { Keys.O, Convert.ToByte("157", 8) },// SI
            { Keys.P, Convert.ToByte("160", 8) },// DLE
            { Keys.Q, Convert.ToByte("161", 8) },// DC1 or XON
            { Keys.R, Convert.ToByte("162", 8) },// DC2
            { Keys.S, Convert.ToByte("163", 8) },// DC3 or XOFF
            { Keys.T, Convert.ToByte("164", 8) },// DC4
            { Keys.U, Convert.ToByte("165", 8) },// NAK
            { Keys.V, Convert.ToByte("166", 8) },// SYN
            { Keys.W, Convert.ToByte("167", 8) },// ETB
            { Keys.X, Convert.ToByte("170", 8) },// CAN
            { Keys.Y, Convert.ToByte("171", 8) },// EM
            { Keys.Z, Convert.ToByte("172", 8) },// SUB
        };

        #endregion

        public override byte TranslateKey(PressedKey key)
        {
            byte translatedByte = 0;

            // 检测非字母Shift键按下
            if (key.IsShiftPressed)
            {
                if (ShiftNonalphabeticKeyCode.TryGetValue(key.Key, out translatedByte))
                {
                    return translatedByte;
                }
            }
            // 检测字母+Shift键按下
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

        public override void ProcessReceivedData(byte[] data)
        {
        }
    }
}