using ModengTerm.Base;

namespace ModengTerm.Addon.Service
{
    public abstract class StorageObject
    {
        /// <summary>
        /// 对象的唯一Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 如果该对象不和任何一个Session关联，写空
        /// </summary>
        public string TabId { get; set; }
    }

    /// <summary>
    /// 对插件提供持久化数据存储服务
    /// </summary>
    public abstract class IClientStorage
    {
        public abstract int AddObject<T>(T obj) where T : StorageObject;

        public int AddObjects<T>(List<T> objects) where T : StorageObject
        {
            foreach (T obj in objects)
            {
                AddObject(obj);
            }

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 获取所有会话里指定类型的所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract List<T> GetObjects<T>() where T : StorageObject;

        /// <summary>
        /// 获取指定会话里指定类型的所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public abstract List<T> GetObjects<T>(string sessionId) where T : StorageObject;

        #region 删除接口

        public abstract int DeleteObject<T>(string objectId) where T : StorageObject;

        /// <summary>
        /// 删除指定会话里指定类型的对象
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public abstract int DeleteObjects<T>(string sessionId) where T : StorageObject;

        /// <summary>
        /// 删除指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int DeleteObject<T>(T obj) where T : StorageObject
        {
            return this.DeleteObject<T>(obj.Id);
        }

        public int DeleteObjects<T>(List<T> objs) where T : StorageObject
        {
            foreach (T obj in objs)
            {
                this.DeleteObject<T>(obj);
            }

            return ResponseCode.SUCCESS;
        }

        #endregion

        public abstract int UpdateObject<T>(T obj) where T : StorageObject;
    }
}
