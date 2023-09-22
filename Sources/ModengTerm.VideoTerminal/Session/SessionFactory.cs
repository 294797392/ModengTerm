using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.Session
{
    public static class SessionFactory
    {
        public static SessionDriver Create(XTermSession options)
        {
            switch ((SessionTypeEnum)options.Type)
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
