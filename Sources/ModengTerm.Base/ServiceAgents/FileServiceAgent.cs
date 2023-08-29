using DotNEToolkit;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;

namespace ModengTerm.Base.ServiceAgents
{
    /// <summary>
    /// 对本体文件数据库进行管理
    /// </summary>
    public class FileServiceAgent : ServiceAgent
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FileServiceAgent");

        protected override int OnInitialize()
        {
            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        public override MTermManifest GetManifest()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.json");

            try
            {
                return JSONHelper.ParseFile<MTermManifest>(path);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("加载AppManifest异常, {0}, {1}", path, ex);
                return default(MTermManifest);
            }
        }

        #region Session管理

        public override int AddSession(XTermSession session)
        {
            try
            {
                JSONDatabase.Insert<XTermSession>(session);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("AddSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override int DeleteSession(string sessionID)
        {
            try
            {
                JSONDatabase.Delete<XTermSession>(v => v.ID == sessionID);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeleteSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override List<XTermSession> GetSessions()
        {
            try
            {
                return JSONDatabase.SelectAll<XTermSession>();
            }
            catch (Exception ex)
            {
                logger.Error("GetSessions异常", ex);
                return new List<XTermSession>();
            }
        }

        public override int UpdateSession(XTermSession session)
        {
            try
            {
                return JSONDatabase.Update<XTermSession>(v => v.ID == session.ID, session);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #endregion

        public override List<Favorites> GetFavorites()
        {
            try
            {
                return JSONDatabase.SelectAll<Favorites>();
            }
            catch (Exception ex)
            {
                logger.Error("GetFavorites异常", ex);
                return new List<Favorites>();
            }
        }

        public override int AddFavorites(Favorites favorites)
        {
            try
            {
                JSONDatabase.Insert<Favorites>(favorites);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("AddFavorites异常", ex);
                return ResponseCode.FAILED;
            }

        }

        public override int DeleteFavorites(string favId)
        {
            try
            {
                JSONDatabase.Delete<Favorites>(v => v.ID == favId);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeleteFavorites异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override int UpdateFavorites(Favorites favorites)
        {
            try
            {
                return JSONDatabase.Update<Favorites>(v => v.ID == favorites.ID, favorites);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateFavorites异常", ex);
                return ResponseCode.FAILED;
            }
        }
    }
}
