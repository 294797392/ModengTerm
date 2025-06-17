using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel.Panel;
using System.Collections.Generic;

namespace ModengTerm.Addon
{
    public class HostFactoryImpl : HostFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("HostFactoryImpl");

        private StorageService storageSvcImpl = new SqliteStorageService();

        public override List<ISidePanel> CreateSidePanels(List<PanelDefinition> definitions)
        {
            List<ISidePanel> sidePanels = new List<ISidePanel>();

            foreach (PanelDefinition definition in definitions)
            {
                SidePanel sidePanel = new SidePanel();
                sidePanel.Definition = definition;
                sidePanel.ID = definition.ID;
                sidePanel.Name = definition.Name;
                sidePanel.IconURI = definition.Icon;
                sidePanels.Add(sidePanel);
            }

            return sidePanels;
        }

        public override List<IOverlayPanel> CreateOverlayPanels(List<PanelDefinition> definitions)
        {
            List<IOverlayPanel> overlayPanels = new List<IOverlayPanel>();

            foreach (PanelDefinition definition in definitions)
            {
                OverlayPanel sidePanel = new OverlayPanel(VTClientUtils.GetPanelContext());
                sidePanel.Definition = definition;
                sidePanel.ID = definition.ID;
                sidePanel.Name = definition.Name;
                sidePanel.IconURI = definition.Icon;
                overlayPanels.Add(sidePanel);
            }

            return overlayPanels;
        }
    }
}
