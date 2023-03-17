using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XTerminal.Base;

namespace XTerminal.VTDefinitions
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

        private static readonly Dictionary<VTKeys, byte[]> ANSIShiftKeyTable = new Dictionary<VTKeys, byte[]>()
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
            { VTKeys.Space, new byte[] { ASCIITable.ESC, (byte)'O', (byte)' ' } }, { VTKeys.Tab, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'I' } }, { VTKeys.Enter, new byte[] { ASCIITable.ESC, (byte)'O', (byte)'M' } },

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

        #endregion

        #region 实例变量

        ///// <summary>
        ///// 当前是否是ApplicationMode
        ///// </summary>
        //private bool isApplicationMode;

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

        #region 构造方法

        public VTKeyboard()
        {
            this.upperCaseCharacter = new byte[1];
            this.SetAnsiMode(true);
            this.SetKeypadMode(false);
            this.SetCursorKeyMode(false);
        }

        #endregion

        #region 实例方法

        private bool IsShiftKeyPressed(VTInputEvent evt)
        {
            return evt.Modifiers.HasFlag(VTModifierKeys.Shift);
        }

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
        public byte[] TranslateInput(VTInputEvent evt)
        {
            if (evt.Key == VTKeys.None)
            {
                return null;
            }

            byte[] bytes = null;

            if (this.isVt52Mode)
            {
                throw new NotImplementedException();
            }
            else
            {
                #region 尝试映射光标键

                if (this.isCursorKeyApplicationMode)
                {
                    if (CursorKeyApplicationMode.TryGetValue(evt.Key, out bytes))
                    {
                        return bytes;
                    }
                }
                else
                {
                    if (CursorKeyNormalMode.TryGetValue(evt.Key, out bytes))
                    {
                        return bytes;
                    }
                }

                #endregion

                #region 尝试映射Keypad

                if (this.isKeypadApplicationMode)
                {
                    if (KeypadApplicationMode.TryGetValue(evt.Key, out bytes))
                    {
                        return bytes;
                    }
                }
                else
                {
                    if (KeypadNormalMode.TryGetValue(evt.Key, out bytes))
                    {
                        return bytes;
                    }
                }

                #endregion

                #region 都没映射成功，使用默认映射

                // 处理按住Shift键的情况
                if (this.IsShiftKeyPressed(evt))
                {
                    if (ANSIShiftKeyTable.TryGetValue(evt.Key, out bytes))
                    {
                        return bytes;
                    }
                }

                // ANSI兼容模式
                if (ANSIKeyTable.TryGetValue(evt.Key, out bytes))
                {
                    // CapsLock打开了，说明输入的是大写字母，把小写字母转成大写字母
                    if (evt.Key >= VTKeys.A && evt.Key <= VTKeys.Z && evt.CapsLock)
                    {
                        upperCaseCharacter[0] = (byte)(bytes[0] - 32);
                        return upperCaseCharacter;
                    }

                    return bytes;
                }

                logger.ErrorFormat("未找到Key - {0}的映射关系", evt.Key);

                #endregion
            }

            return null;
        }

        #endregion
    }
}
