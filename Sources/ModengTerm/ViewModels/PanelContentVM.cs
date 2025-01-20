using ModengTerm.Base.Enumerations;
using System;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 所有PanelContent的基类
    /// </summary>
    public abstract class PanelContentVM : MenuContentVM
    {
        private bool isLoaded;

        /// <summary>
        /// 获取该界面当前是否是显示状态
        /// </summary>
        public bool IsLoaded
        {
            get { return this.isLoaded; }
        }

        public override void OnInitialize()
        {
        }

        public override void OnRelease()
        {
        }

        public override void OnLoaded()
        {
            this.isLoaded = true;
        }

        public override void OnUnload()
        {
            this.isLoaded = false;
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

        #region 实例变量

        private bool readyOnce;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前会话的连接状态
        /// </summary>
        protected SessionStatusEnum SessionStatus { get { return this.OpenedSession.Status; } }

        protected OpenedSessionVM OpenedSession { get; set; }

        #endregion

        #region PanelContentVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.OpenedSession = this.Parameters[KEY_OPENED_SESSION] as OpenedSessionVM;
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            this.RaiseOnReady();
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }

        public override void OnRelease()
        {
            base.OnRelease();
        }

        #endregion

        #region 实例方法

        private void RaiseOnReady() 
        {
            if (this.readyOnce)
            {
                return;
            }

            if (!this.IsLoaded)
            {
                return;
            }

            if (this.SessionStatus != SessionStatusEnum.Connected) 
            {
                return;
            }

            this.OnReady();

            this.readyOnce = true;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        /// <param name="status"></param>
        public virtual void OnStatusChanged(SessionStatusEnum status)
        {
            switch (status)
            {
                case SessionStatusEnum.Connected:
                    {
                        this.RaiseOnReady();
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>
        /// 当连接成功并且显示的时候触发
        /// 只会触发一次
        /// </summary>
        public abstract void OnReady();

        #endregion
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
