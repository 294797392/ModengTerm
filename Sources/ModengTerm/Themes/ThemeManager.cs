using ModengTerm.Base.Enumerations;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal;

namespace ModengTerm.Themes
{
    public static class ThemeManager
    {
        private static Dictionary<string, object> resourceMap = new Dictionary<string, object>();

        public static TResource GetResource<TResource>(string resourceKey) 
        {
            object resource;
            if (!resourceMap.TryGetValue(resourceKey, out resource))
            {
                resource = App.Current.FindResource(resourceKey);
                resourceMap[resourceKey] = resource;
            }

            if (!(resource is TResource))
            {
                throw new InvalidCastException(string.Format("{0}, {1}", resourceKey, typeof(TResource)));
            }

            return (TResource)resource;
        }

        public static ImageSource GetSessionTypeImageSource(SessionTypeEnum sessionType)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.Localhost: return ThemeManager.GetResource<ImageSource>("5022");
                case SessionTypeEnum.SerialPort: return ThemeManager.GetResource<ImageSource>("5024");
                case SessionTypeEnum.SSH: return ThemeManager.GetResource<ImageSource>("5023");
                case SessionTypeEnum.RawTcp: return ThemeManager.GetResource<ImageSource>("5025");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
