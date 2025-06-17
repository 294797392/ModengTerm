using ModengTerm.Addon;
using ModengTerm.Addon.Client;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ModengTerm
{
    public static class VTClientUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTClientUtils");

        private static PanelContext panelContext;

        public static PanelContext GetPanelContext() 
        {
            if (panelContext == null) 
            {
                panelContext = new PanelContext()
                {
                    StorageService = new SqliteStorageService(),
                    HostWindow = Application.Current.MainWindow as IHostWindow
                };
            }

            return panelContext;
        }
    }
}
