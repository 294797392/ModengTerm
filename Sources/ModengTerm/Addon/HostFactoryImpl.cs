using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel;
using System.Windows;

namespace ModengTerm.Addon
{
    public class HostFactoryImpl : HostFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("HostFactoryImpl");

        private StorageService storageSvcImpl = new SqliteStorageService();

        public override IHostSidePanel CreateSidePanel(PanelDefinition definition)
        {
            HostSidePanel sidePanel = new HostSidePanel();
            sidePanel.Definition = definition;
            sidePanel.ID = definition.ID;
            sidePanel.Name = definition.Name;
            sidePanel.IconURI = definition.Icon;
            return sidePanel;
        }
    }
}
