using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;

namespace ModengTerm.Addons
{
    public abstract class AddonBase
    {
        #region 实例变量

        private List<AddonEventTypes> registerEvents;

        #endregion

        #region 属性

        internal AddonDefinition Definition { get; set; }

        #endregion

        #region Internal

        internal void Initialize()
        {
            this.registerEvents = new List<AddonEventTypes>();

            this.OnInitialize();
        }

        internal void Release()
        {
            this.OnRelease();

            // Release
        }

        internal void RaiseEvent(AddonEventTypes ev)
        {
            if (!this.registerEvents.Contains(ev))
            {
                return;
            }

            this.OnEvent(ev);
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

        #region 受保护方法

        protected void RegisterEvent(AddonEventTypes ev)
        {
            if (this.registerEvents.Contains(ev))
            {
                return;
            }

            this.registerEvents.Add(ev);
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();

        /// <summary>
        /// 当插件有事件触发的时候调用
        /// </summary>
        /// <param name="ev"></param>
        protected abstract void OnEvent(AddonEventTypes ev);


        protected abstract void OnSelectedSessionChanged(OpenedSessionVM oldSession, OpenedSessionVM newSession);
        protected abstract void OnSessionStatusChanged(OpenedSessionVM session, SessionStatusEnum oldStatus, SessionStatusEnum newStatus);

        #endregion
    }
}
