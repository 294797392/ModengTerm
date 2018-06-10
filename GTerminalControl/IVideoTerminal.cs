using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalControl
{
    public interface IVideoTerminal
    {
        event Action<object, VTAction, ParseState> Action;

        VTTypeEnum Type { get; }

        IVTStream Stream { get; }

        bool OnKeyDown(KeyEventArgs key, out byte[] data);

        /// <summary>
        /// 解析终端数据流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        void StartParsing();
    }
}