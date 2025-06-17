using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel.Panel;
using System.Collections.Generic;
using System.Windows;

namespace ModengTerm.Addon
{
    public class HostFactoryImpl : HostFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("HostFactoryImpl");

        private StorageService storageSvcImpl = new SqliteStorageService();
        private IHostEventRegistory eventRegistory = new EventRegistoryImpl();

        public HostFactoryImpl()
        {
            
        }

        public override StorageService GetStorageService()
        {
            return storageSvcImpl;
        }

        public override IHostWindow GetHostWindow()
        {
            return Application.Current.MainWindow as IHostWindow;
        }

        public override IHostEventRegistory GetEventRegistory()
        {
            return eventRegistory;
        }

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
                OverlayPanel overlayPanel = new OverlayPanel();
                overlayPanel.Definition = definition;
                overlayPanel.ID = definition.ID;
                overlayPanel.Name = definition.Name;
                overlayPanel.IconURI = definition.Icon;
                overlayPanel.HostFactory = this;
                overlayPanels.Add(overlayPanel);
            }

            return overlayPanels;
        }
    }
}
