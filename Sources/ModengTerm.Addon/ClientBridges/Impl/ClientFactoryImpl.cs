using ModengTerm.Addon.ClientBridges;
using ModengTerm.Addon.ClientBridges.Impl;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using System.Collections.Generic;
using System.Windows;

namespace ModengTerm.Addon
{
    internal class ClientFactoryImpl : ClientBridge
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientFactoryImpl");

        private IClientStorage storageSvcImpl = new SqliteStorageService();
        private IClientEventRegistry eventRegistory = new ClientEventRegistryImpl();

        public ClientFactoryImpl()
        {

        }

        public override IClientStorage GetStorageService()
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
