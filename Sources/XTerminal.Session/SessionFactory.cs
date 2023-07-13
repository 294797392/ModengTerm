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
                case SessionTypeEnum.SSH: return new SSHSession(options);
                case SessionTypeEnum.Win32CommandLine: return new Win32CMDLineSession(options);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
