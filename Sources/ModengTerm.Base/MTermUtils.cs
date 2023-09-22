using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Base
{
    public static class MTermUtils
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("MTermUtils");

        public static List<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static bool IsTerminal(SessionTypeEnum sessionType)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SerialPort:
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.Win32CommandLine:
                    {
                        return true;
                    }

                case SessionTypeEnum.SFTP:
                    {
                        return false;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
