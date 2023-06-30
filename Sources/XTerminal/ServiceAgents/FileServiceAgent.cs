using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;

namespace XTerminal.ServiceAgents
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

        #region Session管理

        public override int AddSession(Base.DataModels.XTermSession session)
        {
            try
            {
                JSONDatabase.Insert<Base.DataModels.XTermSession>(session);

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
                JSONDatabase.Delete<Base.DataModels.XTermSession>(v => v.ID == sessionID);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeleteSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override List<Base.DataModels.XTermSession> GetSessions()
        {
            try
            {
                return JSONDatabase.SelectAll<Base.DataModels.XTermSession>();
            }
            catch (Exception ex)
            {
                logger.Error("GetSessions异常", ex);
                return new List<Base.DataModels.XTermSession>();
            }
        }

        public override int UpdateSession(Base.DataModels.XTermSession session)
        {
            try
            {
                return JSONDatabase.Update<Base.DataModels.XTermSession>(v => v.ID == session.ID, session);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        #endregion
    }
}
