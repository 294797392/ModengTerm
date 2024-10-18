using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using Newtonsoft.Json;
using Renci.SshNet;
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
    /// 离线服务Agent
    /// </summary>
    public class LocalServiceAgent : ServiceAgent
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("LocalServiceAgent");

        private static readonly string DefaultSessionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "defaultSession.json");

        public override int DeletePlayback(string fileId)
        {
            try
            {
                JSONDatabase.Delete<Playback>(v => v.ID == fileId);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeletePlaybackFile异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override List<Playback> GetPlaybacks(string sessionId)
        {
            try
            {
                return JSONDatabase.SelectAll<Playback>();
            }
            catch (Exception ex)
            {
                logger.Error("GetPlaybackFiles异常", ex);
                return new List<Playback>();
            }
        }

        public override int AddPlayback(Playback file)
        {
            try
            {
                JSONDatabase.Insert<Playback>(file);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("AddPlaybackFile异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #region Session管理

        public override XTermSession GetSession(string sessionId)
        {
            try
            {
                return JSONDatabase.Select<XTermSession>(v => v.ID == sessionId);
            }
            catch (Exception ex)
            {
                logger.Error("GetSession异常", ex);
                return null;
            }
        }

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

        #region ShellCommand管理

        public override List<ShellCommand> GetShellCommands(string sessionId)
        {
            try
            {
                return JSONDatabase.SelectAll<ShellCommand>(v => v.SessionId == sessionId);
            }
            catch (Exception ex)
            {
                logger.Error("GetShellCommands异常", ex);
                return new List<ShellCommand>();
            }
        }

        public override int AddShellCommand(ShellCommand shcmd)
        {
            try
            {
                JSONDatabase.Insert<ShellCommand>(shcmd);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("AddShellCommand异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override int DeleteShellCommand(string id)
        {
            try
            {
                JSONDatabase.Delete<ShellCommand>(v => v.ID == id);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex) 
            {
                logger.Error("DeleteShellCommand异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override int UpdateShellCommand(ShellCommand shcmd)
        {
            try
            {
                JSONDatabase.Update<ShellCommand>(v => v.ID == shcmd.ID, shcmd);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("UpdateShellCommand异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #endregion

        #region RecentSession管理

        public override List<RecentlySession> GetRecentSessions()
        {
            try
            {
                return JSONDatabase.SelectAll<RecentlySession>("recent.json");
            }
            catch (Exception ex)
            {
                logger.Error("GetRecentSessions异常", ex);
                return new List<RecentlySession>();
            }
        }

        public override int AddRecentSession(RecentlySession sessionId)
        {
            try
            {
                JSONDatabase.Insert<RecentlySession>("recent.json", sessionId);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("AddRecentSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override int DeleteRecentSession(string sessionId)
        {
            try
            {
                JSONDatabase.Delete<RecentlySession>("recent.json", v => v.ID == sessionId);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeleteRecentSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #endregion

        #region PrivateKey管理

        public override int AddPrivateKey(PrivateKey privateKey)
        {
            try
            {
                JSONDatabase.Insert<PrivateKey>(privateKey);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("AddPrivateKey异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override int DeletePrivateKey(string id)
        {
            try
            {
                JSONDatabase.Delete<PrivateKey>(v => v.ID == id);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeletePrivateKey异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override List<PrivateKey> GetAllPrivateKey()
        {
            try
            {
                return JSONDatabase.SelectAll<PrivateKey>();
            }
            catch (Exception ex)
            {
                logger.Error("GetAllPrivateKey异常", ex);
                return new List<PrivateKey>();
            }
        }

        public override PrivateKey GetPrivateKey(string id)
        {
            try
            {
                return JSONDatabase.Select<PrivateKey>(v => v.ID == id);
            }
            catch (Exception ex)
            {
                logger.Error("GetPrivateKey异常", ex);
                return null;
            }
        }

        #endregion



        //public override List<Favorites> GetFavorites(string sessionId)
        //{
        //    try
        //    {
        //        return JSONDatabase.SelectAll<Favorites>(this.GetFilePath<Favorites>(sessionId));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("GetFavorites异常", ex);
        //        return new List<Favorites>();
        //    }
        //}

        //public override int AddFavorites(Favorites favorites)
        //{
        //    try
        //    {
        //        JSONDatabase.Insert<Favorites>(this.GetFilePath<Favorites>(favorites.SessionID), favorites);

        //        return ResponseCode.SUCCESS;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("AddFavorites异常", ex);
        //        return ResponseCode.FAILED;
        //    }

        //}

        //public override int DeleteFavorites(Favorites favorites)
        //{
        //    try
        //    {
        //        JSONDatabase.Delete<Favorites>(this.GetFilePath<Favorites>(favorites.SessionID), v => v.ID == favorites.ID);

        //        return ResponseCode.SUCCESS;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("DeleteFavorites异常", ex);
        //        return ResponseCode.FAILED;
        //    }
        //}

        #region 实例方法

        #endregion
    }
}
