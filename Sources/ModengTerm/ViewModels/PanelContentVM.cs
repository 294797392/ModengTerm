using System;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 所有PanelContent的基类
    /// </summary>
    public abstract class PanelContentVM : MenuContentVM
    {
        public override void OnInitialize()
        {
        }

        public override void OnRelease()
        {
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }
    }

    /// <summary>
    /// 和会话关联的Panel
    /// </summary>
    public abstract class SessionPanelContentVM : PanelContentVM
    {
        #region 类变量

        public static readonly string KEY_OPENED_SESSION = Guid.NewGuid().ToString();

        #endregion

        #region 属性

        protected OpenedSessionVM OpenedSession { get; set; }

        #endregion

        #region PanelContentVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.OpenedSession = this.Parameters[KEY_OPENED_SESSION] as OpenedSessionVM;
        }

        #endregion

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        /// <param name="status"></param>
        //public abstract void OnStatusChanged(SessionStatusEnum status);
    }

    /// <summary>
    /// 和窗口关联的Panel
    /// </summary>
    public abstract class WindowPanelContentVM : PanelContentVM
    {
        /// <summary>
        /// 当选中的Session改变的时候触发
        /// </summary>
        /// <param name="oldSession">选中之前的Session</param>
        /// <param name="newSession">选中之后的Session</param>
        //public abstract void OnSelectedSessionChanged(OpenedSessionVM oldSession, OpenedSessionVM newSession);

        /// <summary>
        /// 当Session状态改变的时候触发
        /// </summary>
        /// <param name="session">改变状态的Session</param>
        /// <param name="status">会话的新状态</param>
        //public abstract void OnSessionStatusChanged(OpenedSessionVM session, SessionStatusEnum status);
    }
}
