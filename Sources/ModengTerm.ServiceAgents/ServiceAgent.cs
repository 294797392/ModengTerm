using DotNEToolkit.Modular;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Definitions;

namespace ModengTerm.ServiceAgents
{
    /// <summary>
    /// 远程服务器代理
    /// 远程服务器的作用：
    /// 1. 管理分组和Session信息
    /// 2. 用户数据管理，用户登录
    /// </summary>
    public abstract class ServiceAgent
    {
        /// <summary>
        /// 获取app清单配置文件
        /// </summary>
        /// <returns></returns>
        public abstract MTermManifest GetManifest();

        public int Initialize()
        {
            return ResponseCode.SUCCESS;
        }

        public void Release()
        {
        }

        #region Session管理

        /// <summary>
        /// 获取所有的会话列表
        /// </summary>
        /// <returns></returns>
        public abstract List<XTermSession> GetSessions();

        /// <summary>
        /// 增加一个会话
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public abstract int AddSession(XTermSession session);

        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="sessionID">要删除的SessionID</param>
        /// <returns></returns>
        public abstract int DeleteSession(string sessionID);

        /// <summary>
        /// 更新一个会话的信息
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public abstract int UpdateSession(XTermSession session);

        #endregion

        #region Favorites管理

        public abstract List<Favorites> GetFavorites();

        public abstract int AddFavorites(Favorites favorites);

        public abstract int DeleteFavorites(string favId);

        public abstract int UpdateFavorites(Favorites favorites);

        #endregion
    }
}
