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

        public override StorageService GetStorageService()
        {
            return storageSvcImpl;
        }

        public override IHostWindow GetHostWindow()
        {
            return Application.Current.MainWindow as IHostWindow;
            //return windowImpl;
        }

        public override IHostSidePanel CreateSidePanel(PanelDefinition definition)
        {
            SidePanel sidePanel = new SidePanel();
            sidePanel.Definition = definition;
            sidePanel.ID = definition.ID;
            sidePanel.Name = definition.Name;
            sidePanel.IconURI = definition.Icon;
            return sidePanel;
        }
    }
}
