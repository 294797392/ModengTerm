using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GardeniaTerminalCore
{
    public interface IVideoTerminal
    {
        event Action<object, VTAction, ParseState> Action;

        VTTypeEnum Type { get; }

        IVTStream Stream { get; }

        IVTKeyboard Keyboard { get; }

        /// <summary>
        /// 设置终端字符编码方式
        /// </summary>
        Encoding CharacherEncoding { get; }

        bool HandleInputChar(KeyEventArgs key, out byte[] data);

        bool HandleInputWideChar(string wideChar, out byte[] data);
    }
}