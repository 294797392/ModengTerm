using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace XTerminal.UserControls
{
    public static class SessionContentFactory
    {
        public static SessionContent Create(XTermSession session)
        {
            switch ((SessionTypeEnum)session.Type)
            {
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.Win32CommandLine:
                case SessionTypeEnum.SSH:
                    {
                        return new TerminalContentUserControl();
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
