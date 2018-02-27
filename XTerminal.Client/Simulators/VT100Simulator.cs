using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XTerminal.Terminal;

namespace XTerminal.Simulators
{
    /// <summary>
    /// VT100类型的终端模拟器
    /// 1.解析用户输入的原始字符，解析成VT100字符或控制字符，并发送给终端
    /// 2.解析终端发送过来的控制字符或普通字符，并转转成客户端可识别的命令
    /// </summary>
    public class VT100Simulator : AbstractSimulator
    {
        public override void ProcessKeyDown(PressedKey key)
        {
        }

        public override void ProcessReceivedData(byte[] data)
        {
        }

        /// <summary>
        /// 把按住Control+Key的组合键转换成VT100的命令字符
        /// 参考：
        ///     xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
        ///     xterminal/Dependencies/VT100 User Guide/table3-5.html
        /// </summary>
        private byte Key2ControlCode(Key key)
        {
            switch (key)
            {
                case Key.Space: return Convert.ToByte("000", 8); // Space Bar NUL
                case Key.OemOpenBrackets: return Convert.ToByte("033", 8); //[ ESC
                case Key.Oem5: return Convert.ToByte("034", 8); // \ FS
                case Key.Oem6: return Convert.ToByte("035", 8); // ] GS
                case Key.Oem3: return Convert.ToByte("036", 8); // ~ RS
                case Key.OemQuestion: return Convert.ToByte("037", 8); // ? US

                case Key.A: return Convert.ToByte("001", 8); // SOH
                case Key.B: return Convert.ToByte("002", 8); // STX
                case Key.C: return Convert.ToByte("003", 8); // ETX
                case Key.D: return Convert.ToByte("004", 8); // EOT
                case Key.E: return Convert.ToByte("005", 8); // ENQ
                case Key.F: return Convert.ToByte("006", 8); // ACK
                case Key.G: return Convert.ToByte("007", 8); // BELL
                case Key.H: return Convert.ToByte("010", 8); // BS
                case Key.I: return Convert.ToByte("011", 8); // HT
                case Key.J: return Convert.ToByte("012", 8); // LF
                case Key.K: return Convert.ToByte("013", 8); // VT
                case Key.L: return Convert.ToByte("014", 8); // FF
                case Key.M: return Convert.ToByte("015", 8); // CR
                case Key.N: return Convert.ToByte("016", 8); // SO
                case Key.O: return Convert.ToByte("017", 8); // SI
                case Key.P: return Convert.ToByte("020", 8); // DLE
                case Key.Q: return Convert.ToByte("021", 8); // DC1 or XON
                case Key.R: return Convert.ToByte("022", 8); // DC2
                case Key.S: return Convert.ToByte("023", 8); // DC3 or XOFF
                case Key.T: return Convert.ToByte("024", 8); // DC4
                case Key.U: return Convert.ToByte("025", 8); // NAK
                case Key.V: return Convert.ToByte("026", 8); // SYN
                case Key.W: return Convert.ToByte("027", 8); // ETB
                case Key.X: return Convert.ToByte("030", 8); // CAN
                case Key.Y: return Convert.ToByte("031", 8); // EM
                case Key.Z: return Convert.ToByte("032", 8); // SUB

                default:
                    return byte.MaxValue;
            }
        }

        /// <summary>
        /// 把大写或小写字母转成VT100识别的代码（其实就是字符的ascii码）
        /// 参考:
        ///     xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
        ///     xterminal/Dependencies/VT100 User Guide/table3-2.html
        /// </summary>
        private byte Key2CharacterCode(PressedKey key)
        {
            if (key.IsUpperCase)
            {
                switch (key.Key)
                {
                    case Key.A: return Convert.ToByte("101", 8); // SOH
                    case Key.B: return Convert.ToByte("102", 8); // STX
                    case Key.C: return Convert.ToByte("103", 8); // ETX
                    case Key.D: return Convert.ToByte("104", 8); // EOT
                    case Key.E: return Convert.ToByte("105", 8); // ENQ
                    case Key.F: return Convert.ToByte("106", 8); // ACK
                    case Key.G: return Convert.ToByte("107", 8); // BELL
                    case Key.H: return Convert.ToByte("110", 8); // BS
                    case Key.I: return Convert.ToByte("111", 8); // HT
                    case Key.J: return Convert.ToByte("112", 8); // LF
                    case Key.K: return Convert.ToByte("113", 8); // VT
                    case Key.L: return Convert.ToByte("114", 8); // FF
                    case Key.M: return Convert.ToByte("115", 8); // CR
                    case Key.N: return Convert.ToByte("116", 8); // SO
                    case Key.O: return Convert.ToByte("117", 8); // SI
                    case Key.P: return Convert.ToByte("120", 8); // DLE
                    case Key.Q: return Convert.ToByte("121", 8); // DC1 or XON
                    case Key.R: return Convert.ToByte("122", 8); // DC2
                    case Key.S: return Convert.ToByte("123", 8); // DC3 or XOFF
                    case Key.T: return Convert.ToByte("124", 8); // DC4
                    case Key.U: return Convert.ToByte("125", 8); // NAK
                    case Key.V: return Convert.ToByte("126", 8); // SYN
                    case Key.W: return Convert.ToByte("127", 8); // ETB
                    case Key.X: return Convert.ToByte("130", 8); // CAN
                    case Key.Y: return Convert.ToByte("131", 8); // EM
                    case Key.Z: return Convert.ToByte("132", 8); // SUB
                    default:
                        return byte.MaxValue;
                }
            }
            else
            {
                switch (key.Key)
                {
                    case Key.A: return Convert.ToByte("141", 8); // SOH
                    case Key.B: return Convert.ToByte("142", 8); // STX
                    case Key.C: return Convert.ToByte("143", 8); // ETX
                    case Key.D: return Convert.ToByte("144", 8); // EOT
                    case Key.E: return Convert.ToByte("145", 8); // ENQ
                    case Key.F: return Convert.ToByte("146", 8); // ACK
                    case Key.G: return Convert.ToByte("147", 8); // BELL
                    case Key.H: return Convert.ToByte("150", 8); // BS
                    case Key.I: return Convert.ToByte("151", 8); // HT
                    case Key.J: return Convert.ToByte("152", 8); // LF
                    case Key.K: return Convert.ToByte("153", 8); // VT
                    case Key.L: return Convert.ToByte("154", 8); // FF
                    case Key.M: return Convert.ToByte("155", 8); // CR
                    case Key.N: return Convert.ToByte("156", 8); // SO
                    case Key.O: return Convert.ToByte("157", 8); // SI
                    case Key.P: return Convert.ToByte("160", 8); // DLE
                    case Key.Q: return Convert.ToByte("161", 8); // DC1 or XON
                    case Key.R: return Convert.ToByte("162", 8); // DC2
                    case Key.S: return Convert.ToByte("163", 8); // DC3 or XOFF
                    case Key.T: return Convert.ToByte("164", 8); // DC4
                    case Key.U: return Convert.ToByte("165", 8); // NAK
                    case Key.V: return Convert.ToByte("166", 8); // SYN
                    case Key.W: return Convert.ToByte("167", 8); // ETB
                    case Key.X: return Convert.ToByte("170", 8); // CAN
                    case Key.Y: return Convert.ToByte("171", 8); // EM
                    case Key.Z: return Convert.ToByte("172", 8); // SUB
                    default:
                        return byte.MaxValue;
                }
            }
        }

        /// <summary>
        /// 把按住shift键和没按住shift键的非字母字符转换成VT100识别的sdcii码
        /// 参考:
        ///     xterminal/Dependencies/VT100 User Guide/chapter3.html#S3.3.1
        ///     xterminal/Dependencies/VT100 User Guide/table3-3.html
        /// </summary>
        private byte Key2NonalphabeticKeyCode(PressedKey key)
        {
            if (key.IsShiftPressed)
            {
                switch (key.Key)
                {
                    case Key.D1: return Convert.ToByte("061", 8);
                    case Key.D2: return Convert.ToByte("062", 8);
                    case Key.D3: return Convert.ToByte("063", 8);
                    case Key.D4: return Convert.ToByte("064", 8);
                    case Key.D5: return Convert.ToByte("065", 8);
                    case Key.D6: return Convert.ToByte("066", 8);
                    case Key.D7: return Convert.ToByte("067", 8);
                    case Key.D8: return Convert.ToByte("070", 8);
                    case Key.D9: return Convert.ToByte("071", 8);
                    case Key.D0: return Convert.ToByte("060", 8);
                    case Key.OemMinus: return Convert.ToByte("055", 8); // -
                    case Key.OemPlus: return Convert.ToByte("075", 8); // =
                    case Key.OemOpenBrackets: return Convert.ToByte("133", 8); // [
                    case Key.Oem1: return Convert.ToByte("073", 8);// ;
                    case Key.OemQuotes: return Convert.ToByte("047", 8);// '
                    case Key.OemComma: return Convert.ToByte("054", 8);// ,
                    case Key.OemPeriod: return Convert.ToByte("056", 8);// .
                    case Key.OemQuestion: return Convert.ToByte("057", 8);// /
                    case Key.Oem5: return Convert.ToByte("134", 8); // \
                    case Key.Oem3: return Convert.ToByte("140", 8);// `
                    case Key.Oem6: return Convert.ToByte("135", 8);// ]
                    default:
                        return byte.MaxValue;
                }
            }
            else
            {
                switch (key.Key)
                {
                    case Key.D1: return Convert.ToByte("041", 8);
                    case Key.D2: return Convert.ToByte("100", 8);
                    case Key.D3: return Convert.ToByte("043", 8);
                    case Key.D4: return Convert.ToByte("044", 8);
                    case Key.D5: return Convert.ToByte("045", 8);
                    case Key.D6: return Convert.ToByte("136", 8);
                    case Key.D7: return Convert.ToByte("046", 8);
                    case Key.D8: return Convert.ToByte("052", 8);
                    case Key.D9: return Convert.ToByte("050", 8);
                    case Key.D0: return Convert.ToByte("051", 8);
                    case Key.OemMinus: return Convert.ToByte("137", 8); // -
                    case Key.OemPlus: return Convert.ToByte("053", 8); // =
                    case Key.OemOpenBrackets: return Convert.ToByte("173", 8); // [
                    case Key.Oem1: return Convert.ToByte("072", 8);// ;
                    case Key.OemQuotes: return Convert.ToByte("042", 8);// '
                    case Key.OemComma: return Convert.ToByte("074", 8);// ,
                    case Key.OemPeriod: return Convert.ToByte("076", 8);// .
                    case Key.OemQuestion: return Convert.ToByte("077", 8);// /
                    case Key.Oem5: return Convert.ToByte("174", 8); // \
                    case Key.Oem3: return Convert.ToByte("176", 8);// `
                    case Key.Oem6: return Convert.ToByte("175", 8);// ]
                    default:
                        return byte.MaxValue;
                }

            }
        }
    }
}