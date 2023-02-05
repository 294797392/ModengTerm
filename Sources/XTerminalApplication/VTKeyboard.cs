using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XTerminalBase;

namespace XTerminalController
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
    /// https://vt100.net/docs/vt220-rm/table3-5.html
    /// </summary>
    public class VTKeyboard
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTKeyboard");

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

        /// <summary>
        /// 存储不同模式下按键和终端字节流的映射信息
        /// </summary>
        private Keymap keymap;

        #endregion

        #region 构造方法

        public VTKeyboard()
        {
            this.keymap = new DefaultKeymap();

            this.SetAnsiMode(true);
            this.SetKeypadMode(false);
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 判断该按键是否是光标键就
        /// </summary>
        /// <returns></returns>
        public bool IsCursorKey(VTKeys key)
        {
            return key == VTKeys.UpArrow || key == VTKeys.DownArrow || key == VTKeys.LeftArrow || key == VTKeys.RightArrow;
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
                return this.keymap.MapKey(evt);
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


