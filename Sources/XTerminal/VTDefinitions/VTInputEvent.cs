using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.VTDefinitions
{
    /// <summary>
    /// 存储输入事件
    /// 你需要把你使用的GUI框架（WPF,Winform...etc）的键盘事件转换成VTInputEventArgs
    /// </summary>
    public class VTInputEvent
    {
        /// <summary>
        /// 经过输入法处理后的字符串（用户输入的中文字符），如果没有则填null
        /// 在WPF里这个值使用TextInput事件的e.Text属性获取到
        /// </summary>
        public string Text { get; set; }

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
