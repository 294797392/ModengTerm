using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalCore
{
    public interface IVideoTerminal
    {
        event Action<object, VTAction, ParseState> Action;

        VTTypeEnum Type { get; }

        IVTStream Stream { get; }

        IVTKeyboard Keyboard { get; }

        bool HandleKeyDown(KeyEventArgs key, out byte[] data);
    }
}