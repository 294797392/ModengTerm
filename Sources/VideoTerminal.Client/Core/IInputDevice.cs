using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Drawing
{
    public interface IInputDevice
    {
        /// <summary>
        /// 当用户按下按键的时候要触发这个事件
        /// IVideoTerminal：触发事件的VideoTerminal
        /// VTKeys：用户按下的键盘按键
        /// VTModifierKeys：用户按下的控制按键（ctrl，alt...etc）
        /// string：用户输入的中文字符串，如果没有则写null
        /// </summary>
        event Action<IInputDevice, VTInputEvent> InputEvent;
    }
}
