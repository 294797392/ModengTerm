using DotNEToolkit.DataModels;
using ModengTerm.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;

namespace ModengTerm.Addons
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
        public string SessionId { get; set; }
    }

    /// <summary>
    /// 对插件提供持久化数据存储服务
    /// </summary>
    public abstract class StorageService
    {
        public abstract int AddObject<T>(T obj) where T : StorageObject;

        public int AddObjects<T>(List<T> objects) where T : StorageObject
        {
            foreach (T obj in objects)
            {
                this.AddObject(obj);
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

        /// <summary>
        /// 删除指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract int DeleteObject(string objectId);

        public int DeleteObjects(List<string> objectIds)
        {
            foreach (string objectId in objectIds)
            {
                this.DeleteObject(objectId);
            }

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 删除指定会话里指定类型的对象
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public abstract int DeleteObjects<T>(string sessionId) where T : StorageObject;

        public abstract int UpdateObject<T>(T obj) where T : StorageObject;
    }
}
