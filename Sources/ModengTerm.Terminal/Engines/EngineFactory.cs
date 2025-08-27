using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
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
                        Win32ConsoleEngineEnum driver = options.GetOption<Win32ConsoleEngineEnum>(OptionKeyEnum.CMD_DRIVER);

                        switch (driver)
                        {
                            case Win32ConsoleEngineEnum.Win10PseudoConsoleApi:
                                {
                                    return new PseudoConsoleEngine(options);
                                }

                            case Win32ConsoleEngineEnum.winpty:
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
