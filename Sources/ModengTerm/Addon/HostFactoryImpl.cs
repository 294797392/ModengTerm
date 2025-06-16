using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Panel;
using System.Collections.Generic;
using System.Windows;

namespace ModengTerm.Addon
{
    public class HostFactoryImpl : HostFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("HostFactoryImpl");

        private StorageService storageSvcImpl = new SqliteStorageService();

        public override List<IHostPanel> CreatePanels(List<PanelDefinition> definitions)
        {
            List<IHostPanel> panels = new List<IHostPanel>();

            foreach (PanelDefinition definition in definitions)
            {
                HostPanel sidePanel = new HostPanel();
                sidePanel.Definition = definition;
                sidePanel.ID = definition.ID;
                sidePanel.Name = definition.Name;
                sidePanel.IconURI = definition.Icon;
                panels.Add(sidePanel);
            }

            return panels;
        }
    }
}
