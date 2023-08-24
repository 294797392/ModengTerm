using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace XTerminal.Session
{
    public static class SessionFactory
    {
        public static SessionDriver Create(XTermSession options)
        {
            switch ((SessionTypeEnum)options.SessionType)
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
