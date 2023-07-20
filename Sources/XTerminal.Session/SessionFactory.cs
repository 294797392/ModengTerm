using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Sessions;

namespace XTerminal.Session
{
    public static class SessionFactory
    {
        public static SessionBase Create(VTInitialOptions options)
        {
            switch (options.SessionType)
            {
                case SessionTypeEnum.SSH: return new SshNetSession(options);
                case SessionTypeEnum.Win32CommandLine: return new WinptySession(options);
                case SessionTypeEnum.SerialPort: return new SerialPortSession(options);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
