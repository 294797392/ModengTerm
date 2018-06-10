using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GTerminalCore
{
    public class GVTStream : IVTStream
    {
        public bool EOF
        {
            get
            {
                return true;
            }
        }

        public event Action<object, VTStreamState> StatusChanged;

        public byte Read()
        {
            Thread.Sleep(Timeout.Infinite);
            return 0;
        }

        public byte[] Read(int size)
        {
            Thread.Sleep(Timeout.Infinite);
            return new byte[] { };
        }

        public bool Write(byte[] data)
        {
            return true;
        }

        public bool Write(byte data)
        {
            return true;
        }
    }
}