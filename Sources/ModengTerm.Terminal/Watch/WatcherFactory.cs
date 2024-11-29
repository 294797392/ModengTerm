using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class WatcherFactory
    {
        public static AbstractWatcher Create(XTermSession session)
        {
            switch ((SessionTypeEnum)session.Type)
            {
                case SessionTypeEnum.SSH: return new UnixSshWatcher(session);
                case SessionTypeEnum.AdbShell: return new UnixAdbWatcher(session);
                case SessionTypeEnum.Localhost: return new Win32Watcher(session);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
