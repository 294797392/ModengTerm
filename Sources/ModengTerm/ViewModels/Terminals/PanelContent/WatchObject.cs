using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    /// <summary>
    /// 表示一个监控对象
    /// </summary>
    public abstract class WatchObject : SessionPanelContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchObject");

        #endregion

        #region 实例变量

        private Task watchTask;
        private bool isWatch;
        private ManualResetEvent watchEvent;
        private ShellSessionVM shellSession;

        #endregion

        #region 属性

        #endregion

        #region 公开接口

        public override void OnInitialize()
        {
            base.OnInitialize();

        }

        public override void OnRelease()
        {
            this.StopWatch();

            this.watchEvent.Close();

            base.OnRelease();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();
        }

        public override void OnUnload()
        {
            this.watchEvent.Reset();

            base.OnUnload();
        }

        public override void OnReady()
        {
            this.StartWatch();
        }

        #endregion

        #region 实例方法

        private void StartWatch()
        {
            if (this.isWatch)
            {
                return;
            }

            this.isWatch = true;
            this.watchTask = Task.Factory.StartNew(this.WatchTaskProc);
            this.watchEvent.Set();
        }

        private void StopWatch()
        {
            this.isWatch = false;
            this.watchEvent.Set();
        }

        #endregion

        #region 事件处理器

        private void WatchTaskProc()
        {
            WatchFrequencyEnum frequency = this.shellSession.Session.GetOption<WatchFrequencyEnum>(OptionKeyEnum.WATCH_FREQUENCY, VTBaseConsts.DefaultWatchFrequency);
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
                    break;
                }

                Thread.Sleep(updateInterval);
            }

            watcher.Release();
            this.isWatch = false;
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