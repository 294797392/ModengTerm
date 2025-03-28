using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.UserControls.TerminalUserControls;
using System;

namespace ModengTerm
{
    public static class SessionContentFactory
    {
        public static ISessionContent Create(XTermSession session)
        {
            switch ((SessionTypeEnum)session.Type)
            {
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.Localhost:
                case SessionTypeEnum.SSH:
                    {
                        return new ShellSessionUserControl() { Session = session };
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
