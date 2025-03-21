using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;

namespace ModengTerm.Addons
{
    [AddonMetadata]
    public abstract class AddonBase
    {
        #region 实例变量

        #endregion

        #region Internal

        internal void Initialize()
        {
            this.OnInitialize();
        }

        internal void Release()
        {
            this.OnRelease();

            // Release
        }

        /// <summary>
        /// 获取所有的Panel
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal List<object> GetSideWindows()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 当选中的会话改变的时候触发
        /// </summary>
        /// <param name="session"></param>
        internal void SelectedSessionChanged(OpenedSessionVM oldSession, OpenedSessionVM newSession)
        {

        }

        /// <summary>
        /// 当某个会话状态改变的时候触发
        /// </summary>
        /// <param name="session"></param>
        /// <param name="oldStatus"></param>
        /// <param name="newState"></param>
        internal void SessionStatusChanged(OpenedSessionVM session, SessionStatusEnum oldStatus, SessionStatusEnum newState)
        {
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();
        protected abstract void OnSelectedSessionChanged(OpenedSessionVM oldSession, OpenedSessionVM newSession);
        protected abstract void OnSessionStatusChanged(OpenedSessionVM session, SessionStatusEnum oldStatus, SessionStatusEnum newStatus);

        #endregion
    }
}
