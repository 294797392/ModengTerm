using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Enumerations;

namespace ModengTerm.Terminal.Engines
{
    public static class EngineFactory
    {
        public static AbstractEngin Create(XTermSession options)
        {
            switch ((SessionTypeEnum)options.Type)
            {
                case SessionTypeEnum.SSH: return new SshNetEngine(options);
                case SessionTypeEnum.Localhost:
                    {
                        CmdDriverEnum driver = options.GetOption<CmdDriverEnum>(OptionKeyEnum.CMD_DRIVER);

                        switch (driver)
                        {
                            case CmdDriverEnum.Win10PseudoConsoleApi:
                                {
                                    return new PseudoConsoleEngine(options);
                                }

                            case CmdDriverEnum.winpty:
                                {
                                    return new WinptyEngine(options);
                                }

                            default:
                                throw new NotImplementedException();
                        }
                    }
                case SessionTypeEnum.SerialPort: return new SerialPortEngine(options);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
