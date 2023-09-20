using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModengTerm.Base
{
    public static class MTermUtils
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("MTermUtils");

        public static List<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
