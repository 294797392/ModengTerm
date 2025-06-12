using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public class ObjectFactoryImpl : ObjectFactory
    {
        private IWindow windowImpl = new WindowImpl();
        private StorageService storageSvcImpl = new SqliteStorageService();


        public override StorageService GetStorageService()
        {
            return storageSvcImpl;
        }

        public override IWindow GetWindow()
        {
            return windowImpl;
        }
    }
}
