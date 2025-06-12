using DotNEToolkit;
using DotNEToolkit.DataAccess;
using ModengTerm.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public class JsonDbStorageService : StorageService
    {
        private DBManager dbManager;

        public JsonDbStorageService() 
        {
            this.dbManager = new DBManager("", ProviderType.Sqlite);
            this.dbManager.TestConnection();
        }

        public override int AddObject<T>(string sessionId, T obj)
        {
            throw new NotImplementedException();
        }

        public override int AddObject<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override int AddObjects<T>(string sessionId, IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        public override int DeleteObject(string sessionId, string objectId)
        {
            throw new NotImplementedException();
        }

        public override int DeleteObject<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public override int DeleteObjects(string sessionId, IEnumerable<string> objectIds)
        {
            throw new NotImplementedException();
        }

        public override int DeleteObjects(string sessionId)
        {
            throw new NotImplementedException();
        }

        public override List<T> GetObjects<T>(string sessionId)
        {
            throw new NotImplementedException();
        }

        public override List<T> GetObjects<T>()
        {
            throw new NotImplementedException();
        }

        public override int UpdateObject<T>(string sessionId, T obj)
        {
            throw new NotImplementedException();
        }

        public override int UpdateObject<T>(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
