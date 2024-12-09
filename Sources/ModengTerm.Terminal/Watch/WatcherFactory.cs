using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Session;

namespace ModengTerm.Terminal.Watch
{
    public abstract class WatcherFactory
    {
        public static AbstractWatcher Create(SessionTransport sessionTransport)
        {
            XTermSession session = sessionTransport.Session;
            SessionDriver driver = sessionTransport.Driver;

            switch ((SessionTypeEnum)sessionTransport.Session.Type)
            {
                case SessionTypeEnum.SSH: return new UnixSshWatcher(session, driver);
                case SessionTypeEnum.AdbShell: return new UnixAdbWatcher(session, driver);
                case SessionTypeEnum.Localhost: return new Win32Watcher(session, driver);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
