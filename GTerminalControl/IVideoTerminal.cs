using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalControl
{
    public interface IVideoTerminal
    {
        event Action<object, byte, byte[]> Action;

        VTTypeEnum Type { get; }

        bool OnKeyDown(KeyEventArgs key, out byte[] data);

        bool ParseTerminalStream(byte[] stream);
    }
}