using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 键盘钩子
    /// 同一时刻只能存在一个钩子
    /// </summary>
    public interface IKeyboardHook
    {
        /// <summary>
        /// 当键盘上有按键被按下的时候触发
        /// </summary>
        /// <param name="vtKey">被按下的按键</param>
        /// <returns>如果要阻止继续处理按键事件，返回false，否则返回true</returns>
        bool OnKeyDown(VTKeys vtKey);
    }
}
