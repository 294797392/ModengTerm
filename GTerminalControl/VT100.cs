using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalControl
{
    public class VT100 : IVideoTerminal
    {
        public event Action<object, byte, byte[]> Action;

        public VTTypeEnum Type
        {
            get
            {
                return VTTypeEnum.VT100;
            }
        }

        public bool OnKeyDown(KeyEventArgs key, out byte[] data)
        {
            throw new NotImplementedException();
        }

        public bool Parse(byte[] stream)
        {
            throw new NotImplementedException();
        }
    }
}