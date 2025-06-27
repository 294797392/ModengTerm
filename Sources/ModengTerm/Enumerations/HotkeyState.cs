using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Enumerations
{
    public enum HotkeyState
    {
        /// <summary>
        /// 表示按下的第一个按键
        /// </summary>
        Key1,

        /// <summary>
        /// 表示按下的第二个按键
        /// </summary>
        Key2,

        /// <summary>
        /// 处理双快捷键的情况
        /// Ctrl+A,B
        /// </summary>
        DoubleHotkey,

        /// <summary>
        /// 处理双修饰键的情况
        /// </summary>
        DoubleModKey,
    }

}
