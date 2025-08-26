using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Engines;

namespace ModengTerm.Terminal.Watch
{
    public abstract class WatcherFactory
    {
        public static AbstractWatcher Create(EngineTransport sessionTransport)
        {
            XTermSession session = sessionTransport.Session;
            AbstractEngin driver = sessionTransport.Engine;

            switch ((SessionTypeEnum)sessionTransport.Session.Type)
            {
                case SessionTypeEnum.SSH: return new UnixSshWatcher(session, driver);
                case SessionTypeEnum.Localhost:
                    {
                        bool enableAdb = session.GetOption<bool>(OptionKeyEnum.WATCH_ADB_ENABLED, OptionDefaultValues.WATCH_ADB_ENABLED);
                        if (enableAdb)
                        {
                            return new UnixAdbWatcher(session, driver);
                        }
                        else
                        {
                            return new Win32Watcher(session, driver);
                        }
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
