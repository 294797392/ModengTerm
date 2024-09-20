using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XTerminal.Base;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 把不同模式下的键盘按键转换成要发送给终端的字节序列
    /// 
    /// The VT100 is an upward and downward software compatible terminal; that is, previous DIGITAL video terminals have DIGITAL private standards for control sequences. The American National Standards Institute (ANSI) has since standardized escape and control sequences in terminals in documents X3.41-1974 and X3.64-1977.
    /// NOTE: The ANSI standards allow the manufacturer flexibility in implementing each function.This manual describes how the VT100 will respond to the implemented ANSI control function.
    /// The VT100 is compatible with both the previous DIGITAL standard and ANSI standards. Customers may use existing DIGITAL software designed around the VT52 or new VT100 software.The VT100 has a "VT52 compatible" mode in which the VT100 responds to control sequences like a VT52. In this mode, most of the new VT100 features cannot be used.
    /// Throughout this section of the manual, references will be made to "VT52 mode" or "ANSI mode". These two terms are used to indicate the VT100's software compatibility. All new software should be designed around the VT100 "ANSI mode". Future DIGITAL video terminals will not necessarily be committed to VT52 compatibility.
    /// 
    /// VT100兼容旧的VT52模式下的控制序列和ANSI控制序列，所以VT100有两种模式，一种是VT52控制序列模式，一种是ANSI控制序列模式
    /// 
    /// 参考：
    /// terminalInput.cpp
    /// VT100 User Guide/Chapter 3 - Programmer Information/The Keyboard
    /// </summary>
    public class VTKeyboard
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTKeyboard");

        private static readonly Dictionary<VTKeys, byte[]> ANSIKeyTable = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Escape, new byte[] { 0x1B } },

            { VTKeys.A, new byte[] { (byte)'a' } }, { VTKeys.B, new byte[] { (byte)'b' } }, { VTKeys.C, new byte[] { (byte)'c' } }, { VTKeys.D, new byte[] { (byte)'d' } },
            { VTKeys.E, new byte[] { (byte)'e' } }, { VTKeys.F, new byte[] { (byte)'f' } }, { VTKeys.G, new byte[] { (byte)'g' } }, { VTKeys.H, new byte[] { (byte)'h' } },
            { VTKeys.I, new byte[] { (byte)'i' } }, { VTKeys.J, new byte[] { (byte)'j' } }, { VTKeys.K, new byte[] { (byte)'k' } }, { VTKeys.L, new byte[] { (byte)'l' } },
            { VTKeys.M, new byte[] { (byte)'m' } }, { VTKeys.N, new byte[] { (byte)'n' } }, { VTKeys.O, new byte[] { (byte)'o' } }, { VTKeys.P, new byte[] { (byte)'p' } },
            { VTKeys.Q, new byte[] { (byte)'q' } }, { VTKeys.R, new byte[] { (byte)'r' } }, { VTKeys.S, new byte[] { (byte)'s' } }, { VTKeys.T, new byte[] { (byte)'t' } },
            { VTKeys.U, new byte[] { (byte)'u' } }, { VTKeys.V, new byte[] { (byte)'v' } }, { VTKeys.W, new byte[] { (byte)'w' } }, { VTKeys.X, new byte[] { (byte)'x' } },
            { VTKeys.Y, new byte[] { (byte)'y' } }, { VTKeys.Z, new byte[] { (byte)'z' } },

            // FunctionKeys - VT100 User Guide/Chapter 3 - Programmer Information/The Keyboard
            { VTKeys.Enter, new byte[] { (byte)'\n' } }, { VTKeys.Space, new byte[] { (byte)' ' } },
            { VTKeys.Back, new byte[] { 8 } }, { VTKeys.Tab, new byte[] { 9 } },


            { VTKeys.OemOpenBrackets, new byte[] { (byte)'[' } }, { VTKeys.OemCloseBrackets, new byte[] { (byte)']' } },{ VTKeys.Oem5, new byte[] { (byte)'\\' } },
            { VTKeys.Oem1, new byte[] { (byte)';' } }, { VTKeys.OemQuotes, new byte[] { (byte)'\'' } },
            { VTKeys.OemComma, new byte[] { (byte)',' } }, { VTKeys.OemPeriod, new byte[] { (byte)'.' } }, { VTKeys.OemQuestion, new byte[] { (byte)'/' } },

            // 上面的数字键
            { VTKeys.Oem3, new byte[] { (byte)'`' } }, { VTKeys.D1, new byte[] { (byte)'1' } }, { VTKeys.D2, new byte[] { (byte)'2' } },
            { VTKeys.D3, new byte[] { (byte)'3' } }, { VTKeys.D4, new byte[] { (byte)'4' } },{ VTKeys.D5, new byte[] { (byte)'5' } }, { VTKeys.D6, new byte[] { (byte)'6' } },
            { VTKeys.D7, new byte[] { (byte)'7' } }, { VTKeys.D8, new byte[] { (byte)'8' } },{ VTKeys.D9, new byte[] { (byte)'9' } }, { VTKeys.D0, new byte[] { (byte)'0' } },
            { VTKeys.OemMinus, new byte[] { (byte)'-' } }, { VTKeys.OemPlus, new byte[] { (byte)'=' } },
        };

        private static readonly Dictionary<VTKeys, byte[]> ANSIKeyShiftPressed = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.A, new byte[] { (byte)'a' } }, { VTKeys.B, new byte[] { (byte)'b' } }, { VTKeys.C, new byte[] { (byte)'c' } }, { VTKeys.D, new byte[] { (byte)'d' } },
            { VTKeys.E, new byte[] { (byte)'e' } }, { VTKeys.F, new byte[] { (byte)'f' } }, { VTKeys.G, new byte[] { (byte)'g' } }, { VTKeys.H, new byte[] { (byte)'h' } },
            { VTKeys.I, new byte[] { (byte)'i' } }, { VTKeys.J, new byte[] { (byte)'j' } }, { VTKeys.K, new byte[] { (byte)'k' } }, { VTKeys.L, new byte[] { (byte)'l' } },
            { VTKeys.M, new byte[] { (byte)'m' } }, { VTKeys.N, new byte[] { (byte)'n' } }, { VTKeys.O, new byte[] { (byte)'o' } }, { VTKeys.P, new byte[] { (byte)'p' } },
            { VTKeys.Q, new byte[] { (byte)'q' } }, { VTKeys.R, new byte[] { (byte)'r' } }, { VTKeys.S, new byte[] { (byte)'s' } }, { VTKeys.T, new byte[] { (byte)'t' } },
            { VTKeys.U, new byte[] { (byte)'u' } }, { VTKeys.V, new byte[] { (byte)'v' } }, { VTKeys.W, new byte[] { (byte)'w' } }, { VTKeys.X, new byte[] { (byte)'x' } },
            { VTKeys.Y, new byte[] { (byte)'y' } }, { VTKeys.Z, new byte[] { (byte)'z' } },

            { VTKeys.OemOpenBrackets, new byte[] { (byte)'{' } }, { VTKeys.OemCloseBrackets, new byte[] { (byte)'}' } },{ VTKeys.Oem5, new byte[] { (byte)'|' } },
            { VTKeys.Oem1, new byte[] { (byte)':' } }, { VTKeys.OemQuotes, new byte[] { (byte)'"' } },
            { VTKeys.OemComma, new byte[] { (byte)'<' } }, { VTKeys.OemPeriod, new byte[] { (byte)'>' } }, { VTKeys.OemQuestion, new byte[] { (byte)'?' } },

            { VTKeys.Oem3, new byte[] { (byte)'~' } },{ VTKeys.D1, new byte[] { (byte)'!' } }, { VTKeys.D2, new byte[] { (byte)'@' } },
            { VTKeys.D3, new byte[] { (byte)'#' } }, { VTKeys.D4, new byte[] { (byte)'$' } }, { VTKeys.D5, new byte[] { (byte)'%' } }, { VTKeys.D6, new byte[] { (byte)'^' } },
            { VTKeys.D7, new byte[] { (byte)'&' } }, { VTKeys.D8, new byte[] { (byte)'*' } }, { VTKeys.D9, new byte[] { (byte)'(' } }, { VTKeys.D0, new byte[] { (byte)')' } },
            { VTKeys.OemMinus, new byte[] { (byte)'_' } }, { VTKeys.OemPlus, new byte[] { (byte)'+' } },
        };

        #region Control键按下的时候字母键映射

        private static readonly Dictionary<VTKeys, byte[]> ANSIKeyControlPressed = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Space, new byte[] { 0 } },

            { VTKeys.A, new byte[] { 1 } }, { VTKeys.B, new byte[] { 2 } }, { VTKeys.C, new byte[] { 3 } }, { VTKeys.D, new byte[] { 4 } },
            { VTKeys.E, new byte[] { 5 } }, { VTKeys.F, new byte[] { 6 } }, { VTKeys.G, new byte[] { 7 } }, { VTKeys.H, new byte[] { 8 } },
            { VTKeys.I, new byte[] { 9 } }, { VTKeys.J, new byte[] { 10 } }, { VTKeys.K, new byte[] { 11 } }, { VTKeys.L, new byte[] { 12 } },
            { VTKeys.M, new byte[] { 13 } }, { VTKeys.N, new byte[] { 14 } }, { VTKeys.O, new byte[] { 15 } }, { VTKeys.P, new byte[] { 16 } },
            { VTKeys.Q, new byte[] { 17 } }, { VTKeys.R, new byte[] { 18 } }, { VTKeys.S, new byte[] { 19 } }, { VTKeys.T, new byte[] { 20 } },
            { VTKeys.U, new byte[] { 21 } }, { VTKeys.V, new byte[] { 22 } }, { VTKeys.W, new byte[] { 23 } }, { VTKeys.X, new byte[] { 24 } },
            { VTKeys.Y, new byte[] { 25 } }, { VTKeys.Z, new byte[] { 26 } },
        };

        #endregion

        #region 方向键映射

        private static readonly Dictionary<VTKeys, byte[]> CursorKeyVT52 = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Up, new byte[] { ASCIITable.ESC, (byte)'A' } }, { VTKeys.Down, new byte[] { ASCIITable.ESC, (byte)'B' } },
            { VTKeys.Right, new byte[] { ASCIITable.ESC, (byte)'C' } }, { VTKeys.Left, new byte[] { ASCIITable.ESC, (byte)'D' } },
        };

        private static readonly Dictionary<VTKeys, byte[]> CursorKeyNormalMode = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Up, new byte[] { ASCIITable.ESC, (byte)'[', (byte)'A' } }, { VTKeys.Down, new byte[] { ASCIITable.ESC,  (byte)'[', (byte)'B' } },
            { VTKeys.Right, new byte[] { ASCIITable.ESC, (byte)'[', (byte)'C' } }, { VTKeys.Left, new byte[] { ASCIITable.ESC,  (byte)'[', (byte)'D' } },
        };

        private static readonly Dictionary<VTKeys, byte[]> CursorKeyApplicationMode = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Up, new byte[] { ASCIITable.ESC,  (byte)'O', (byte)'A' } }, { VTKeys.Down, new byte[] { ASCIITable.ESC,  (byte)'O', (byte)'B' } },
            { VTKeys.Right, new byte[] { ASCIITable.ESC,  (byte)'O', (byte)'C' } }, { VTKeys.Left, new byte[] { ASCIITable.ESC,  (byte)'O', (byte)'D' } },
        };

        /// <summary>
        /// 不管CursorKey当前是ApplicationMode还是NormalMode，只要Control键按下了，那么就使用这个映射关系
        /// </summary>
        private static readonly Dictionary<VTKeys, byte[]> CursorKeyControlPressed = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Up, new byte[] { ASCIITable.ESC,  (byte)'[', (byte)'1', (byte)'5', (byte)'A' } }, { VTKeys.Down, new byte[] { ASCIITable.ESC,  (byte)'[', (byte)'1', (byte)'5', (byte)'B' } },
            { VTKeys.Right, new byte[] { ASCIITable.ESC,  (byte)'[', (byte)'1', (byte)'5', (byte)'C' } }, { VTKeys.Left, new byte[] { ASCIITable.ESC,  (byte)'[', (byte)'1', (byte)'5', (byte)'D' } },
        };

        #endregion

        #region 辅助键盘映射

        private static readonly Dictionary<VTKeys, byte[]> VT52AuxiliaryKeyApplicationModeTable = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.NumPad0, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'p' } }, { VTKeys.NumPad1, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'1' } },
            { VTKeys.NumPad2, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'r' } }, { VTKeys.NumPad3, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'s' } },
            { VTKeys.NumPad4, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'t' } }, { VTKeys.NumPad5, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'u' } },
            { VTKeys.NumPad6, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'v' } }, { VTKeys.NumPad7, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'w' } },
            { VTKeys.NumPad8, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'x' } }, { VTKeys.NumPad9, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'y' } },

            // dash
            { VTKeys.Subtract, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'m' } },
            // period
            { VTKeys.Decimal, new byte[] { ASCIITable.ESC, (byte)'?', (byte)'n' } }
        };

        private static readonly Dictionary<VTKeys, byte[]> VT52AuxiliaryKeyNumericModeTable = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.NumPad0, new byte[] { (byte)'0' } }, { VTKeys.NumPad1, new byte[] { (byte)'1' } },
            { VTKeys.NumPad2, new byte[] { (byte)'2' } }, { VTKeys.NumPad3, new byte[] { (byte)'3' } },
            { VTKeys.NumPad4, new byte[] { (byte)'4' } }, { VTKeys.NumPad5, new byte[] { (byte)'5' } },
            { VTKeys.NumPad6, new byte[] { (byte)'6' } }, { VTKeys.NumPad7, new byte[] { (byte)'7' } },
            { VTKeys.NumPad8, new byte[] { (byte)'8' } }, { VTKeys.NumPad9, new byte[] { (byte)'9' } },

            // dash
            { VTKeys.Subtract, new byte[] { (byte)'-' } },
            // period
            { VTKeys.Decimal, new byte[] { (byte)'.' } }
        };

        private static readonly Dictionary<VTKeys, byte[]> KeypadNormalMode = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Space, new byte[] { (byte)' ' } }, { VTKeys.Tab, new byte[] { (byte)'\t' } }, { VTKeys.Enter, new byte[] { (byte)'\r' } },
            { VTKeys.NumPad0, new byte[] { (byte)'0' } }, { VTKeys.NumPad1, new byte[] { (byte)'1' } },
            { VTKeys.NumPad2, new byte[] { (byte)'2' } }, { VTKeys.NumPad3, new byte[] { (byte)'3' } },
            { VTKeys.NumPad4, new byte[] { (byte)'4' } }, { VTKeys.NumPad5, new byte[] { (byte)'5' } },
            { VTKeys.NumPad6, new byte[] { (byte)'6' } }, { VTKeys.NumPad7, new byte[] { (byte)'7' } },
            { VTKeys.NumPad8, new byte[] { (byte)'8' } }, { VTKeys.NumPad9, new byte[] { (byte)'9' } },

            // dash
            { VTKeys.Subtract, new byte[] { (byte)'-' } },
            // period
            { VTKeys.Decimal, new byte[] { (byte)'.' } }
        };

        private static readonly Dictionary<VTKeys, byte[]> KeypadApplicationMode = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.Space, new byte[] { (byte)' ' } }, { VTKeys.Tab, new byte[] { (byte)'\t' } }, { VTKeys.Enter, new byte[] { (byte)'\r' } },

            { VTKeys.NumPad0, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'p' } }, { VTKeys.NumPad1, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'1' } },
            { VTKeys.NumPad2, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'r' } }, { VTKeys.NumPad3, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'s' } },
            { VTKeys.NumPad4, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'t' } }, { VTKeys.NumPad5, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'u' } },
            { VTKeys.NumPad6, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'v' } }, { VTKeys.NumPad7, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'w' } },
            { VTKeys.NumPad8, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'x' } }, { VTKeys.NumPad9, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'y' } },

            // dash
            { VTKeys.Subtract, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'m' } },
            // period
            { VTKeys.Decimal, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'n' } }
        };

        #endregion

        #region EditingKeypad

        public static readonly Dictionary<VTKeys, byte[]> EditingKeypad = new Dictionary<VTKeys, byte[]>()
        {
            // insert
            { VTKeys.Insert, Encoding.ASCII.GetBytes("\x1b[2~") },
            { VTKeys.Delete, Encoding.ASCII.GetBytes("\x1b[3~") },
            // page up
            { VTKeys.PageUp, Encoding.ASCII.GetBytes("\x1b[5~") },
            // page down
            { VTKeys.Next, Encoding.ASCII.GetBytes("\x1b[6~") },

            { VTKeys.Home, Encoding.ASCII.GetBytes("\x1b[1~") },
            { VTKeys.End, Encoding.ASCII.GetBytes("\x1b[4~") },
        };

        #endregion

        #region F1 - F12

        private static readonly Dictionary<VTKeys, byte[]> FunctionKeysTable = new Dictionary<VTKeys, byte[]>()
        {
            //// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html#h3-PC-Style-Function-Keys
            //{ VTKeys.F1, Encoding.ASCII.GetBytes("\x1bOP~") }, // SS1 P
            //{ VTKeys.F2, Encoding.ASCII.GetBytes("\x1bOQ~") }, // SS1 Q
            //{ VTKeys.F3, Encoding.ASCII.GetBytes("\x1bOR~") }, // SS1 R
            //{ VTKeys.F4, Encoding.ASCII.GetBytes("\x1bOS~") }, // SS1 S

            { VTKeys.F1, Encoding.ASCII.GetBytes("\x1b[11~") }, // CSI 1 1 ~
            { VTKeys.F2, Encoding.ASCII.GetBytes("\x1b[12~") }, // CSI 1 2 ~
            { VTKeys.F3, Encoding.ASCII.GetBytes("\x1b[13~") }, // CSI 1 3 ~
            { VTKeys.F4, Encoding.ASCII.GetBytes("\x1b[14~") }, // CSI 1 4 ~


            { VTKeys.F5, Encoding.ASCII.GetBytes("\x1b[15~") },
            { VTKeys.F6, Encoding.ASCII.GetBytes("\x1b[17~") },
            { VTKeys.F7, Encoding.ASCII.GetBytes("\x1b[18~") },
            { VTKeys.F8, Encoding.ASCII.GetBytes("\x1b[19~") },
            { VTKeys.F9, Encoding.ASCII.GetBytes("\x1b[20~") },
            { VTKeys.F10, Encoding.ASCII.GetBytes("\x1b[21~") },
            { VTKeys.F11, Encoding.ASCII.GetBytes("\x1b[23~") },
            { VTKeys.F12, Encoding.ASCII.GetBytes("\x1b[24~") }
        };

        #endregion

        #endregion

        #region 实例变量

        /// <summary>
        /// 当前是否按照VT52模式来解析终端序列
        /// </summary>
        private bool isVt52Mode;

        /// <summary>
        /// 键盘是否是Application模式
        /// 1. NumbericMode
        /// 2. ApplicationMode
        /// VT52模式下的2种模式和ANSI模式下的2种模式发送的控制序列不一样
        /// </summary>
        private bool isKeypadApplicationMode;

        /// <summary>
        /// 光标键的模式（就是上下左右键）
        /// 仅在终端是VT52模式下并且键盘是ApplicationMode下才生效
        /// 如果是cursorKeyMode：发送ANSI光标控制命令
        /// 如果不是cursorKeyMode：发送Application控制命令
        /// </summary>
        private bool isCursorKeyApplicationMode;

        private byte[] upperCaseCharacter;

        #endregion

        #region 属性

        /// <summary>
        /// 翻译使用的编码方式
        /// 默认值是XTermConsts.DefaultInputEncoding
        /// </summary>
        public Encoding Encoding { get; set; }

        #endregion

        #region 构造方法

        public VTKeyboard()
        {
            this.upperCaseCharacter = new byte[1];
            this.SetAnsiMode(true);
            this.SetKeypadMode(false);
            this.SetCursorKeyMode(false);
            this.Encoding = Encoding.GetEncoding(MTermConsts.DefaultWriteEncoding);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        /// <summary>
        /// 设置当前的光标键模式是否是ApplicationMode
        /// 光标键有两种模式，一种模式是ApplicationMode，另外一种是NormalMode
        /// 光标键就是上下左右键
        /// </summary>
        /// <param name="isApplicationMode"></param>
        public void SetKeypadMode(bool isApplicationMode)
        {
            this.isKeypadApplicationMode = isApplicationMode;
        }

        /// <summary>
        /// 设置当前终端解析数据流的模式
        /// </summary>
        /// <param name="isAnsiMode"></param>
        public void SetAnsiMode(bool isAnsiMode)
        {
            this.isVt52Mode = !isAnsiMode;
        }

        public void SetCursorKeyMode(bool isApplicationMode)
        {
            this.isCursorKeyApplicationMode = isApplicationMode;
        }

        /// <summary>
        /// 把系统按键转换成终端字节序列
        /// 代码参考terminal - terminalInput.cpp
        /// </summary>
        /// <returns></returns>
        public byte[] TranslateInput(VTKeyInput userInput)
        {
            byte[] bytes = null;

            if (this.isVt52Mode)
            {
                throw new NotImplementedException();
            }
            else
            {
                #region 尝试映射光标键

                if (userInput.Modifiers.HasFlag(VTModifierKeys.Control))
                {
                    if (CursorKeyControlPressed.TryGetValue(userInput.Key, out bytes))
                    {
                        return bytes;
                    }
                }
                else
                {
                    if (this.isCursorKeyApplicationMode)
                    {
                        if (CursorKeyApplicationMode.TryGetValue(userInput.Key, out bytes))
                        {
                            return bytes;
                        }
                    }
                    else
                    {
                        if (CursorKeyNormalMode.TryGetValue(userInput.Key, out bytes))
                        {
                            return bytes;
                        }
                    }
                }

                #endregion

                #region 尝试映射Keypad

                if (this.isKeypadApplicationMode)
                {
                    if (KeypadApplicationMode.TryGetValue(userInput.Key, out bytes))
                    {
                        return bytes;
                    }
                }
                else
                {
                    if (KeypadNormalMode.TryGetValue(userInput.Key, out bytes))
                    {
                        return bytes;
                    }
                }

                #endregion

                #region 尝试映射EditingKeypad

                if (EditingKeypad.TryGetValue(userInput.Key, out bytes))
                {
                    return bytes;
                }

                #endregion

                #region 尝试映射F1 - F12

                if (FunctionKeysTable.TryGetValue(userInput.Key, out bytes))
                {
                    return bytes;
                }

                #endregion

                #region 都没映射成功，使用默认映射

                // 处理按住Shift键的情况
                if (userInput.Modifiers.HasFlag(VTModifierKeys.Shift))
                {
                    if (ANSIKeyShiftPressed.TryGetValue(userInput.Key, out bytes))
                    {
                        return bytes;
                    }
                }

                // 处理按住Control键的情况
                if (userInput.Modifiers.HasFlag(VTModifierKeys.Control)) 
                {
                    if (ANSIKeyControlPressed.TryGetValue(userInput.Key, out bytes))
                    {
                        return bytes;
                    }
                }

                // ANSI兼容模式
                if (ANSIKeyTable.TryGetValue(userInput.Key, out bytes))
                {
                    // CapsLock打开了，说明输入的是大写字母，把小写字母转成大写字母
                    if (userInput.Key >= VTKeys.A && userInput.Key <= VTKeys.Z && userInput.CapsLock)
                    {
                        upperCaseCharacter[0] = (byte)(bytes[0] - 32);
                        return upperCaseCharacter;
                    }

                    return bytes;
                }

                logger.ErrorFormat("未找到Key - {0}的映射关系", userInput.Key);

                #endregion
            }

            return null;
        }

        #endregion
    }
}
