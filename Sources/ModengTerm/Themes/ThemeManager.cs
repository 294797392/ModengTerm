using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Themes
{
    public static class ThemeManager
    {
        private static Dictionary<string, object> resourceMap = new Dictionary<string, object>();
        private static Dictionary<string, ResourceDictionary> resDictMap = new Dictionary<string, ResourceDictionary>();
        private static ResourceDictionary currentTheme;

        public static TResource GetResource<TResource>(string resourceKey) where TResource : Freezable
        {
            object resource;
            if (!resourceMap.TryGetValue(resourceKey, out resource))
            {
                resource = App.Current.FindResource(resourceKey);
                resourceMap[resourceKey] = resource;

                Freezable freezable = resource as Freezable;
                if (freezable != null) 
                {
                    //使用Freeze可以提高性能
                    freezable.Freeze();
                }
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
                case SessionTypeEnum.LocalConsole: return ThemeManager.GetResource<ImageSource>("501");
                case SessionTypeEnum.SerialPort: return ThemeManager.GetResource<ImageSource>("503");
                case SessionTypeEnum.Ssh: return ThemeManager.GetResource<ImageSource>("502");
                case SessionTypeEnum.Tcp: return ThemeManager.GetResource<ImageSource>("504");
                default:
                    throw new NotImplementedException();
            }
        }

        public static ResourceDictionary GetResourceDictionary(string resourceUri)
        {
            ResourceDictionary resourceDictionary;
            if (!resDictMap.TryGetValue(resourceUri, out resourceDictionary))
            {
                resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri(resourceUri);
                resDictMap[resourceUri] = resourceDictionary;
            }
            return resourceDictionary;
        }

        /// <summary>
        /// 应用主题
        /// </summary>
        public static void ApplyTheme(string uri)
        {
            ResourceDictionary resourceDictionary = GetResourceDictionary(uri);

            if (currentTheme == resourceDictionary)
            {
                return;
            }

            if (currentTheme != null) 
            {
                Application.Current.Resources.MergedDictionaries.Remove(currentTheme);
            }

            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

            currentTheme = resourceDictionary;

            resourceMap.Clear();
        }
    }
}
