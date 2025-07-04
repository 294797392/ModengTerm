using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
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
        private IClientEventRegistry eventRegistory = new ClientEventRegistryImpl();

        public ClientFactoryImpl()
        {

        }

        public override StorageService GetStorageService()
        {
            return storageSvcImpl;
        }

        public override IClient GetClient()
        {
            return Application.Current.MainWindow as IClient;
        }

        public override IClientEventRegistry GetEventRegistry()
        {
            return eventRegistory;
        }
    }
}
