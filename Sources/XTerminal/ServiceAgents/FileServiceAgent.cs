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

        public override int AddSession(SessionDM session)
        {
            try
            {
                JSONDatabase.Insert<SessionDM>(session);

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
                JSONDatabase.Delete<SessionDM>(v => v.ID == sessionID);

                return ResponseCode.SUCCESS;
            }
            catch (Exception ex)
            {
                logger.Error("DeleteSession异常", ex);
                return ResponseCode.FAILED;
            }
        }

        public override List<SessionDM> GetSessions()
        {
            try
            {
                return JSONDatabase.SelectAll<SessionDM>();
            }
            catch (Exception ex)
            {
                logger.Error("GetSessions异常", ex);
                return new List<SessionDM>();
            }
        }

        public override int UpdateSession(SessionDM session)
        {
            try
            {
                return JSONDatabase.Update<SessionDM>(v => v.ID == session.ID, session);
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
