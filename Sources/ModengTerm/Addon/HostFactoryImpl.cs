using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addon
{
    public class HostFactoryImpl : HostFactory
    {
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
    }
}
