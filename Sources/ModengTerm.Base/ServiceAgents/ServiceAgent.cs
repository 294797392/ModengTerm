using ModengTerm.Base.DataModels;

namespace ModengTerm.Base.ServiceAgents
{
    /// <summary>
    /// 远程服务器代理
    /// 远程服务器的作用：
    /// 1. 管理分组和Session信息
    /// 2. 用户数据管理，用户登录
    /// </summary>
    public abstract class ServiceAgent
    {
        public int Initialize()
        {
            //this.cryptor = Cryptor.Create(SecretKeyTypeEnum.AES256_Local);
            //this.cryptor.Initialize();

            return ResponseCode.SUCCESS;
        }

        public void Release()
        {
            //this.cryptor.Release();
        }

        #region Session管理

        public abstract XTermSession GetSession(string sessionId);

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

        #region SessionGroup管理

        public abstract List<SessionGroup> GetSessionGroups();
        public abstract int DeleteSessionGroup(string groupId);
        public abstract int AddSessionGroup(SessionGroup sessionGroup);

        #endregion

        #region Playback管理

        public abstract List<Playback> GetPlaybacks(string sessionId);
        public abstract int AddPlayback(Playback file);
        public abstract int DeletePlayback(string fileId);

        #endregion

        #region ShellCommand管理

        public abstract List<ShellCommand> GetShellCommands(string sessionId);
        public abstract int AddShellCommand(ShellCommand shcmd);
        public abstract int DeleteShellCommand(string id);
        public abstract int UpdateShellCommand(ShellCommand shcmd);

        #endregion

        #region RecentSession管理

        public abstract List<RecentlySession> GetRecentSessions();
        public abstract int AddRecentSession(RecentlySession session);
        public abstract int DeleteRecentSession(string sessionId);

        #endregion

        #region PrivateKey管理

        public abstract int AddPrivateKey(PrivateKey privateKey);
        public abstract int DeletePrivateKey(string id);
        public abstract List<PrivateKey> GetAllPrivateKey();
        public abstract PrivateKey GetPrivateKey(string id);

        #endregion

        //protected string EncryptObject(object toEncrypt)
        //{
        //    return this.cryptor.Encrypt(toEncrypt);
        //}

        //protected T DecryptObject<T>(string toDecrypt)
        //{
        //    return this.cryptor.Decrypt<T>(toDecrypt);
        //}

        //public abstract List<Favorites> GetFavorites(string sessionId);
        //public abstract int AddFavorites(Favorites favorites);
        //public abstract int DeleteFavorites(Favorites favorites);
    }
}
