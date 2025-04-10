﻿using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 所有PanelContent的基类
    /// TODO：尝试把PanelContentVM改造成插件
    /// </summary>
    public abstract class PanelContentVM : MenuContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PanelContentVM");

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        #endregion

        #region MenuContentVM

        public override void OnInitialize()
        {
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }

        public override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region 抽象方法

        #endregion
    }

    public abstract class SessionPanelContentVM : MenuContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SessionPanelContentVM");

        #endregion

        #region 实例变量

        private bool isLoaded;
        private bool readyOnce;
        private SessionStatusEnum sessionStatus;

        #endregion

        #region 属性

        /// <summary>
        /// 获取该窗口所关联的原始Session数据
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 获取该窗口所关联的会话
        /// </summary>
        public OpenedSessionVM OpenedSessionVM { get; set; }

        /// <summary>
        /// 获取访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        #endregion

        #region MenuContentVM

        static int id = 0;

        public override void OnInitialize()
        {
            this.ID = ++id;

            this.sessionStatus = this.OpenedSessionVM.Status;
        }

        public override void OnLoaded()
        {
            logger.InfoFormat("{0} OnLoaded", this.ID);

            this.isLoaded = true;

            this.RaiseOnReady();
        }

        public override void OnUnload()
        {
            logger.InfoFormat("{0} OnUnload", this.ID);

            this.isLoaded = false;
        }

        public override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        private void RaiseOnReady()
        {
            if (this.readyOnce)
            {
                return;
            }

            if (!this.isLoaded)
            {
                return;
            }

            if (this.sessionStatus != SessionStatusEnum.Connected)
            {
                return;
            }

            this.OnReady();

            this.readyOnce = true;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 问题点：没点击菜单的时候，实例还没被创建，如何触发
        /// 会话状态改变的时候触发
        /// </summary>
        /// <param name="status"></param>
        public void OnSessionStatusChanged(SessionStatusEnum status)
        {
            this.sessionStatus = status;

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
        /// 当以下两个条件全部成立的时候触发：
        /// 1. 会话状态是连接成功
        /// 2. 页面当前是显示状态
        /// 只会触发一次
        /// </summary>
        public abstract void OnReady();

        #endregion
    }
}
