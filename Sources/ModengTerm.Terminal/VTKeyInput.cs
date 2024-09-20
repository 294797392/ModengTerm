using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 存储按键输入事件
    /// 你需要把你使用的GUI框架（WPF,Winform...etc）的键盘事件转换成VTInputEventArgs
    /// </summary>
    public class VTKeyInput
    {
        /// <summary>
        /// 用户按下的按键
        /// </summary>
        public VTKeys Key { get; set; }

        /// <summary>
        /// 用户按下的修饰键，如果有多个修饰键，则按位存储
        /// </summary>
        public VTModifierKeys Modifiers { get; set; }

        /// <summary>
        /// CapsLock按键的状态
        /// </summary>
        public bool CapsLock { get; set; }
    }
}
