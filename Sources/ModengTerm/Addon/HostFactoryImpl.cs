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

        public override List<IHostOverlayPanel> CreateOverlayPanels(List<PanelDefinition> definitions)
        {
            throw new System.NotImplementedException();
        }

        public override List<IHostSidePanel> CreateSidePanels(List<PanelDefinition> definitions)
        {
            List<IHostSidePanel> sidePanels = new List<IHostSidePanel>();

            foreach (PanelDefinition definition in definitions)
            {
                HostSidePanel sidePanel = new HostSidePanel();
                sidePanel.Definition = definition;
                sidePanel.ID = definition.ID;
                sidePanel.Name = definition.Name;
                sidePanel.IconURI = definition.Icon;
                sidePanels.Add(sidePanel);
            }

            return sidePanels;
        }
    }
}
