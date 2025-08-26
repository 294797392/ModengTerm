using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Enumerations;

namespace ModengTerm.Terminal.Session
{
    public static class SessionFactory
    {
        public static SessionDriver Create(XTermSession options)
        {
            switch ((SessionTypeEnum)options.Type)
            {
                case SessionTypeEnum.SSH: return new SshNetSession(options);
                case SessionTypeEnum.Localhost:
                    {
                        CmdDriverEnum driver = options.GetOption<CmdDriverEnum>(OptionKeyEnum.CMD_DRIVER);

                        switch (driver)
                        {
                            case CmdDriverEnum.Win10PseudoConsoleApi:
                                {
                                    return new PseudoConsoleSession(options);
                                }

                            case CmdDriverEnum.winpty:
                                {
                                    return new WinptySession(options);
                                }

                            default:
                                throw new NotImplementedException();
                        }
                    }
                case SessionTypeEnum.SerialPort: return new SerialPortSession(options);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
