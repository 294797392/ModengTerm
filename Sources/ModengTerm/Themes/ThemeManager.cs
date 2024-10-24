﻿using System;
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
    }
}
