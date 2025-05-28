using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    public abstract class AddonObjectStorage
    {
        public abstract int AddObject<T>(string sessionId, T obj);

        public abstract int AddObjects<T>(string sessionId, IEnumerable<T> objs);

        public abstract List<T> GetObjects<T>(string sessionId);

        public abstract int DeleteObject(string sessionId, string objectId);

        public abstract int DeleteObjects(string sessionId, IEnumerable<string> objectIds);

        public abstract int UpdateObject<T>(string sessionId, T obj);



        public abstract int AddObject<T>(T obj);

        public abstract List<T> GetObjects<T>();

        public abstract int DeleteObject<T>(T obj);

        public abstract int UpdateObject<T>(T obj);



        /// <summary>
        /// 删除所有和会话关联的对象
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public abstract int DeleteObjects(string sessionId);
    }
}
