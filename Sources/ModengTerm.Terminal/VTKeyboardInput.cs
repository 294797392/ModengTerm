using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 存储通过键盘输入的数据
    /// 你需要把你使用的GUI框架（WPF,Winform...etc）的键盘事件转换成VTKeyInput
    /// </summary>
    public class VTKeyboardInput
    {
        private string text;

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

        public string Text 
        {
            get { return this.text; }
            set
            {
                if (this.text != value) 
                {
                    this.text = value;
                }
            }
        }

        /// <summary>
        /// 是否是通过输入法输入的数据
        /// </summary>
        public bool FromIMEInput { get; set; }

        /// <summary>
        /// 经过转换之后的要发送的数据
        /// </summary>
        public byte[] SendBytes { get; set; }

        public VTKeyboardInput() 
        {
        }
    }
}
