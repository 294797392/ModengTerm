using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Terminal.Enumerations;

namespace ModengTerm.Terminal.Engines
{
    public static class ChannelFactory
    {
        /// <summary>
        /// 创建会话通道实例
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ChannelBase Create(ChannelOptions options)
        {
            switch (options.SessionType)
            {
                case SessionTypeEnum.Ssh:
                    {
                        return new SshNetChannel();
                    }

                case SessionTypeEnum.Console:
                    {
                        LocalConsoleChannelOptions channelOptions = options as LocalConsoleChannelOptions;

                        switch (channelOptions.ConsoleEngin)
                        {
                            case Win32ConsoleEngineEnum.Auto:
                                {
                                    if (VTBaseUtils.IsWin10())
                                    {
                                        return new PseudoConsoleChannel();
                                    }
                                    else
                                    {
                                        return new WinptyChannel();
                                    }
                                }

                            case Win32ConsoleEngineEnum.Win10PseudoConsoleApi:
                                {
                                    return new PseudoConsoleChannel();
                                }

                            case Win32ConsoleEngineEnum.winpty:
                                {
                                    return new WinptyChannel();
                                }

                            default:
                                throw new NotImplementedException();
                        }
                    }

                case SessionTypeEnum.SerialPort:
                    {
                        return new SerialPortChannel();
                    }

                case SessionTypeEnum.Tcp:
                    {
                        return new TcpChannel();
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
