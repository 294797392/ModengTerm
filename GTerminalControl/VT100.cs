using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalControl
{
    public class VT100 : IVideoTerminal
    {
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
    }
}