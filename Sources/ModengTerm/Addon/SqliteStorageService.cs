using DotNEToolkit;
using DotNEToolkit.DataAccess;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Xceed.Wpf.Toolkit;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 使用Sqlite实现本地存储服务
    /// </summary>
    public class SqliteStorageService : IClientStorage
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("SqliteStorageService");
        private const string DbFileResourceName = "ModengTerm.database.db";
        private const string ConnectionStringFormat = "Data Source={0};Version=3;";

        private DBManager dbManager;

        public SqliteStorageService()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.db");
            this.EnsureDbFile(filePath);
            string connectionString = string.Format(ConnectionStringFormat, filePath);
            this.dbManager = new DBManager(connectionString, ProviderType.Sqlite);
        }

        /// <summary>
        /// 确保db文件存在
        /// </summary>
        /// <param name="filePath"></param>
        private void EnsureDbFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(DbFileResourceName))
                {
                    byte[] fileBytes = new byte[stream.Length];
                    stream.Read(fileBytes, 0, fileBytes.Length);
                    File.WriteAllBytes(filePath, fileBytes);
                }
            }
        }

        public override int AddObject<T>(T obj)
        {
            DbParameter[] dbParameters = dbManager.CreateDbParameters(4);

            dbParameters[0].DbType = System.Data.DbType.String;
            dbParameters[0].ParameterName = "id";
            dbParameters[0].Value = obj.Id;

            dbParameters[1].DbType = System.Data.DbType.String;
            dbParameters[1].ParameterName = "sid";
            dbParameters[1].Value = obj.TabId;

            dbParameters[2].DbType = System.Data.DbType.String;
            dbParameters[2].ParameterName = "type";
            dbParameters[2].Value = typeof(T).FullName;

            dbParameters[3].DbType = System.Data.DbType.String;
            dbParameters[3].ParameterName = "data";
            dbParameters[3].Value = JsonConvert.SerializeObject(obj);

            try
            {
                int rc = dbManager.ExecuteNonQuery("insert into object(id,sessionId,type,data) values(@id,@sid,@type,@data);", System.Data.CommandType.Text, dbParameters);
                if (rc < 1)
                {
                    logger.ErrorFormat("AddObject失败, {0}", rc);
                    return ResponseCode.FAILED;
                }

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("AddObject异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #region 删除接口

        public override int DeleteObject<T>(string objectId)
        {
            DbParameter[] dbParameters = dbManager.CreateDbParameters(1);

            dbParameters[0].DbType = System.Data.DbType.String;
            dbParameters[0].ParameterName = "id";
            dbParameters[0].Value = objectId;

            try
            {
                int rc = dbManager.ExecuteNonQuery("delete from object where id=@id;", System.Data.CommandType.Text, dbParameters);
                if (rc < 1)
                {
                    logger.ErrorFormat("DeleteObject失败, {0}", rc);
                    return ResponseCode.FAILED;
                }

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeleteObject异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override int DeleteObjects<T>(string sessionId)
        {
            DbParameter[] dbParameters = dbManager.CreateDbParameters(2);

            dbParameters[0].DbType = System.Data.DbType.String;
            dbParameters[0].ParameterName = "sid";
            dbParameters[0].Value = sessionId;

            dbParameters[1].DbType = System.Data.DbType.String;
            dbParameters[1].ParameterName = "type";
            dbParameters[1].Value = typeof(T).FullName;

            try
            {
                int rc = dbManager.ExecuteNonQuery("delete from object where sessionId=@sid and type=@type;", System.Data.CommandType.Text, dbParameters);
                if (rc < 1)
                {
                    logger.ErrorFormat("DeleteObjects失败, {0}", rc);
                    return ResponseCode.FAILED;
                }

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeleteObjects异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #endregion

        public override List<T> GetObjects<T>()
        {
            DbParameter[] dbParameters = dbManager.CreateDbParameters(1);

            dbParameters[0].DbType = System.Data.DbType.String;
            dbParameters[0].ParameterName = "type";
            dbParameters[0].Value = typeof(T).FullName;

            DataSet ds = null;

            try
            {
                ds = dbManager.ExecuteDataSet("select * from object where type=@type", System.Data.CommandType.Text, dbParameters);
            }
            catch (Exception ex)
            {
                logger.Error("GetObjects异常", ex);
                return null;
            }

            List<T> objects = new List<T>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                T obj = JsonConvert.DeserializeObject<T>(row["data"].ToString());
                objects.Add(obj);
            }

            return objects;
        }

        public override List<T> GetObjects<T>(string sessionId)
        {
            DbParameter[] dbParameters = dbManager.CreateDbParameters(2);

            dbParameters[0].DbType = System.Data.DbType.String;
            dbParameters[0].ParameterName = "sid";
            dbParameters[0].Value = sessionId;

            dbParameters[1].DbType = System.Data.DbType.String;
            dbParameters[1].ParameterName = "type";
            dbParameters[1].Value = typeof(T).FullName;

            DataSet ds = null;

            try
            {
                ds = dbManager.ExecuteDataSet("select * from object where sessionId=@sid and type=@type", System.Data.CommandType.Text, dbParameters);
            }
            catch (Exception ex)
            {
                logger.Error("GetObjects异常", ex);
                return null;
            }

            List<T> objects = new List<T>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                T obj = JsonConvert.DeserializeObject<T>(row["data"].ToString());
                objects.Add(obj);
            }

            return objects;
        }

        public override int UpdateObject<T>(T obj)
        {
            DbParameter[] dbParameters = dbManager.CreateDbParameters(2);

            dbParameters[0].DbType = System.Data.DbType.String;
            dbParameters[0].ParameterName = "id";
            dbParameters[0].Value = obj.Id;

            dbParameters[1].DbType = System.Data.DbType.String;
            dbParameters[1].ParameterName = "data";
            dbParameters[1].Value = JsonConvert.SerializeObject(obj);

            try
            {
                int rc = dbManager.ExecuteNonQuery("update object set data=@data where id=@id;", System.Data.CommandType.Text, dbParameters);
                if (rc < 1)
                {
                    logger.ErrorFormat("UpdateObject失败, {0}", rc);
                    return ResponseCode.FAILED;
                }

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("UpdateObject异常", ex);
                return ResponseCode.FAILED;
            }
        }
    }
}
