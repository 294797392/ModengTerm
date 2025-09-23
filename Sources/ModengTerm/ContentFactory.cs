using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.UserControls.FtpUserControls;
using ModengTerm.UserControls.TerminalUserControls;
using System;

namespace ModengTerm
{
    public static class ContentFactory
    {
        public static ISessionContent Create(XTermSession session)
        {
            switch ((SessionTypeEnum)session.Type)
            {
                case SessionTypeEnum.Tcp:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.Console:
                case SessionTypeEnum.Ssh:
                    {
                        return new ShellSessionUserControl();
                    }

                case SessionTypeEnum.Sftp:
                    {
                        return new FtpSessionUserControl();
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
