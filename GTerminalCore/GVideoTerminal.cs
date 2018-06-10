using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalCore
{
    public class GVideoTerminal : VideoTerminal
    {
        public override VTTypeEnum Type
        {
            get
            {
                return VTTypeEnum.GVT;
            }
        }

        public override bool OnKeyDown(KeyEventArgs key, out byte[] data)
        {
            data = new byte[] { };
            return false;
        }
    }
}