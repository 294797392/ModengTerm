using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XTerminalBase;

namespace XTerminalDevice
{
    //public enum KeyboardMaps
    //{
    //    /// <summary>
    //    /// 普通模式下的光标键
    //    /// </summary>
    //    CursorKeyNormal = 0,

    //    /// <summary>
    //    /// Application模式下的光标键
    //    /// </summary>
    //    CursorKeyApplication,

    //    /// <summary>
    //    /// VT52模式下的光标键
    //    /// </summary>
    //    CursorKeyVT52,

    //    /// <summary>
    //    /// VT52模式下的按键
    //    /// </summary>
    //    KeypadVT52,

    //    /// <summary>
    //    /// 普通模式下的按键
    //    /// </summary>
    //    KeypadNormal,

    //    /// <summary>
    //    /// Application模式下的按键
    //    /// </summary>
    //    KeypadApplicaion,
    //}

    /// <summary>
    /// 把不同模式下的键盘按键转换成要发送给终端的字节序列
    /// 参考：
    /// terminalInput.cpp
    /// VT100 User Guide/Chapter 3 - Programmer Information/The Keyboard
    /// </summary>
    public class VTKeyboard
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTKeyboard");

        private static readonly Dictionary<VTKeys, byte[]> Key2BytesTable = new Dictionary<VTKeys, byte[]>()
        {
            { VTKeys.A, new byte[] { (byte)'a' } }, { VTKeys.B, new byte[] { (byte)'b' } }, { VTKeys.C, new byte[] { (byte)'c' } }, { VTKeys.D, new byte[] { (byte)'d' } },
            { VTKeys.E, new byte[] { (byte)'e' } }, { VTKeys.F, new byte[] { (byte)'f' } }, { VTKeys.G, new byte[] { (byte)'g' } }, { VTKeys.H, new byte[] { (byte)'h' } },
            { VTKeys.I, new byte[] { (byte)'i' } }, { VTKeys.J, new byte[] { (byte)'j' } }, { VTKeys.K, new byte[] { (byte)'k' } }, { VTKeys.L, new byte[] { (byte)'l' } },
            { VTKeys.M, new byte[] { (byte)'m' } }, { VTKeys.N, new byte[] { (byte)'n' } }, { VTKeys.O, new byte[] { (byte)'o' } }, { VTKeys.P, new byte[] { (byte)'p' } },
            { VTKeys.Q, new byte[] { (byte)'q' } }, { VTKeys.R, new byte[] { (byte)'r' } }, { VTKeys.S, new byte[] { (byte)'s' } }, { VTKeys.T, new byte[] { (byte)'t' } },
            { VTKeys.U, new byte[] { (byte)'u' } }, { VTKeys.V, new byte[] { (byte)'v' } }, { VTKeys.W, new byte[] { (byte)'w' } }, { VTKeys.X, new byte[] { (byte)'x' } },
            { VTKeys.Y, new byte[] { (byte)'y' } }, { VTKeys.Z, new byte[] { (byte)'z' } },

            // FunctionKeys - VT100 User Guide/Chapter 3 - Programmer Information/The Keyboard
            { VTKeys.Enter, new byte[] { (byte)'\n' } }, { VTKeys.Space, new byte[] { (byte)' ' } }, { VTKeys.Back, new byte[] { } },
            { VTKeys.Back, new byte[] { 8 } }, { VTKeys.Tab, new byte[] { 9 } },


            { VTKeys.OemOpenBrackets, new byte[] { (byte)'[' } }, { VTKeys.OemCloseBrackets, new byte[] { (byte)']' } },{ VTKeys.Oem5, new byte[] { (byte)'|' } },
            { VTKeys.Oem1, new byte[] { (byte)';' } }, { VTKeys.OemQuotes, new byte[] { (byte)'\'' } },
            { VTKeys.OemComma, new byte[] { (byte)',' } }, { VTKeys.OemPeriod, new byte[] { (byte)'.' } }, { VTKeys.OemQuestion, new byte[] { (byte)'/' } },

            // 上面的数字键
            { VTKeys.Oem3, new byte[] { (byte)'`' } }, { VTKeys.D1, new byte[] { (byte)'1' } }, { VTKeys.D2, new byte[] { (byte)'2' } },
            { VTKeys.D3, new byte[] { (byte)'3' } }, { VTKeys.D4, new byte[] { (byte)'4' } },{ VTKeys.D5, new byte[] { (byte)'5' } }, { VTKeys.D6, new byte[] { (byte)'6' } },
            { VTKeys.D7, new byte[] { (byte)'7' } }, { VTKeys.D8, new byte[] { (byte)'8' } },{ VTKeys.D9, new byte[] { (byte)'9' } }, { VTKeys.D0, new byte[] { (byte)'0' } },
            { VTKeys.OemMinus, new byte[] { (byte)'-' } }, { VTKeys.OemPlus, new byte[] { (byte)'+' } },
        };

        #endregion

        #region 实例变量

        /// <summary>
        /// 当前是否是ApplicationMode
        /// </summary>
        private bool isApplicationMode;

        /// <summary>
        /// 当前是否是VT52模式
        /// </summary>
        private bool isVt52Mode;

        private byte[] capitalBytes;

        #endregion

        #region 构造方法

        public VTKeyboard()
        {
            this.capitalBytes = new byte[1];
            this.SetAnsiMode(true);
            this.SetKeypadMode(false);
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 判断该按键是否是光标键就
        /// </summary>
        /// <returns></returns>
        private bool IsCursorKey(VTKeys key)
        {
            return key == VTKeys.Up || key == VTKeys.Down || key == VTKeys.Left || key == VTKeys.Right;
        }

        private byte[] MapKey(VTInputEvent evt)
        {
            byte[] bytes;
            if (!Key2BytesTable.TryGetValue(evt.Key, out bytes))
            {
                logger.ErrorFormat("未找到Key - {0}的映射关系", evt.Key);
                return null;
            }

            // 这里表示输入的是大写字母
            if (evt.Key >= VTKeys.A && evt.Key <= VTKeys.Z && evt.CapsLock)
            {
                capitalBytes[0] = (byte)(bytes[1] - 32);
                return capitalBytes;
            }

            return bytes;
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
            this.isApplicationMode = isApplicationMode;
        }

        /// <summary>
        /// 设置当前终端解析数据流的模式
        /// </summary>
        /// <param name="isAnsiMode"></param>
        public void SetAnsiMode(bool isAnsiMode)
        {
            this.isVt52Mode = !isAnsiMode;
        }

        /// <summary>
        /// 把系统按键转换成终端字节序列
        /// 代码参考terminal - terminalInput.cpp
        /// </summary>
        /// <returns></returns>
        public byte[] TranslateInput(VTInputEvent evt)
        {
            if (evt.Key != VTKeys.None)
            {
                return this.MapKey(evt);
            }

            return null;

            //if (mkey == VTModifierKeys.None)
            //{
            // 没有按下修饰键

            //if (this.isVt52Mode)
            //{
            //    // VT52模式下的按键转换，VT52模式下没有Application模式
            //    if (this.IsCursorKey(key))
            //    {
            //        keyText = this.keymap[KeyboardMaps.CursorKeyVT52][key];
            //    }
            //    else
            //    {
            //        keyText = this.keymap[KeyboardMaps.KeypadVT52][key];
            //    }
            //}
            //else
            //{
            //    // ANSI模式下的按键转换
            //    if (this.IsCursorKey(key))
            //    {
            //        // 按下的是光标键
            //        if (this.isApplicationMode)
            //        {
            //            // 获取Application模式下的光标键的要发送的字节序列
            //            keyText = this.keymap[KeyboardMaps.CursorKeyApplication][key];
            //        }
            //        else
            //        {
            //            // 获取Normal模式下的光标键的要发送的字节序列
            //            keyText = this.keymap[KeyboardMaps.CursorKeyNormal][key];
            //        }
            //    }
            //    else
            //    {
            //        // 按下的是其他按键
            //        if (this.isApplicationMode)
            //        {
            //            keyText = this.keymap[KeyboardMaps.KeypadApplicaion][key];
            //        }
            //        else
            //        {
            //            keyText = this.keymap[KeyboardMaps.KeypadNormal][key];
            //        }
            //    }
            //}
            //}
            //else
            //{
            //    // 按下了修饰键，有可能一次按了多个修饰键
            //}

            // 如果keyText里包含转义字符，那么解析转义字符，转义字符使用十六进制表示，用\x或者\0开头

            //return Encoding.ASCII.GetBytes(keyText);
        }

        #endregion
    }
}
