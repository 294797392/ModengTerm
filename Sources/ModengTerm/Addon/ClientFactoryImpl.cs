using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel.Panel;
using System.Collections.Generic;
using System.Windows;

namespace ModengTerm.Addon
{
    public class ClientFactoryImpl : ClientFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientFactoryImpl");

        private StorageService storageSvcImpl = new SqliteStorageService();
        private IClientEventRegistory eventRegistory = new ClientEventRegistoryImpl();

        public ClientFactoryImpl()
        {

        }

        public override StorageService GetStorageService()
        {
            return storageSvcImpl;
        }

        public override IClientWindow GetHostWindow()
        {
            return Application.Current.MainWindow as IClientWindow;
        }

        public override IClientEventRegistory GetEventRegistory()
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

        public override IOverlayPanel CreateOverlayPanel(PanelDefinition definition)
        {
            OverlayPanel overlayPanel = new OverlayPanel();
            overlayPanel.Definition = definition;
            overlayPanel.ID = definition.ID;
            overlayPanel.Name = definition.Name;
            overlayPanel.IconURI = definition.Icon;
            overlayPanel.HostFactory = this;
            return overlayPanel;
        }
    }
}
