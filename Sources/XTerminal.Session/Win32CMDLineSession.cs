using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace XTerminal.Session
{
    public class Win32CMDLineSession : SessionBase
    {
        public Win32CMDLineSession(VTInitialOptions options) : 
            base(options)
        { }

        public override int Connect()
        {
            return ResponseCode.SUCCESS;
        }

        public override void Disconnect()
        {
        }

        public override int Write(byte[] data)
        {
            return ResponseCode.SUCCESS;
        }

        protected override int OnInitialize()
        {
            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }
    }
}
