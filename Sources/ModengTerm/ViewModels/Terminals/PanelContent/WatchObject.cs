using log4net.Repository.Hierarchy;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    /// <summary>
    /// 表示一个监控对象
    /// </summary>
    public abstract class WatchObject : PanelContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchObject");

        #endregion

        #region 实例变量

        private Task watchTask;
        private bool isWatch;
        private ManualResetEvent watchEvent;
        private ShellSessionVM shellSession;
        private ServiceAgent serviceAgent;

        #endregion

        #region 属性

        #endregion

        #region 公开接口

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.shellSession = this.Parameters[PanelContentVM.KEY_OPENED_SESSION] as ShellSessionVM;
            this.serviceAgent = this.Parameters[PanelContentVM.KEY_SERVICE_AGENT] as ServiceAgent;
            this.watchEvent = new ManualResetEvent(false);
        }

        public override void OnRelease()
        {
            this.isWatch = false;
            this.watchEvent.Reset();

            base.OnRelease();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            if (this.watchTask == null)
            {
                this.isWatch = true;
                this.watchTask = Task.Factory.StartNew(this.WatchTaskProc);
            }

            this.watchEvent.Set();
        }

        public override void OnUnload()
        {
            this.watchEvent.Reset();

            base.OnUnload();
        }

        #endregion

        #region 事件处理器

        private void WatchTaskProc()
        {
            WatchFrequencyEnum frequency = this.shellSession.Session.GetOption<WatchFrequencyEnum>(OptionKeyEnum.WATCH_FREQUENCY, MTermConsts.DefaultWatchFrequency);
            int updateInterval = VTBaseUtils.GetWatchInterval(frequency);
            List<WatchObject> watchList = new List<WatchObject>();
            AbstractWatcher watcher = WatcherFactory.Create(this.shellSession.Transport);
            watcher.Initialize();

            while (this.isWatch)
            {
                try
                {
                    this.watchEvent.WaitOne();

                    this.Watch(watcher);
                }
                catch (Exception ex)
                {
                    logger.Error("WatchThread异常", ex);
                }

                Thread.Sleep(updateInterval);
            }

            watcher.Release();
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 执行定时任务
        /// </summary>
        public abstract void Watch(AbstractWatcher watcher);

        #endregion
    }
}
