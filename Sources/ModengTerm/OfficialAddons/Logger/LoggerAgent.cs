using DotNEToolkit;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModengTerm.OfficialAddons.Logger
{
    /// <summary>
    /// 用来记录日志的单例类
    /// </summary>
    public class LoggerAgent : SingletonObject<LoggerAgent>
    {
        #region 类变量

        private static log4net.ILog log4netLogger = log4net.LogManager.GetLogger("LoggerAddon");

        /// <summary>
        /// 写入日志文件的间隔时间，单位是毫秒
        /// </summary>
        private const int WriteInterval = 3000;

        #endregion

        #region 实例变量

        private IClientEventRegistry eventRegistry;

        private Thread writeThread;
        private ManualResetEvent loggerEvent;
        private bool initOnce = false;
        private List<LoggerVM> loggerList;
        private List<LoggerVM> loggerListCopy;
        private bool listChanged;
        private object listLock;

        #endregion

        public LoggerAgent()
        {
            ClientFactory clientFactory = ClientFactory.GetFactory();
            this.eventRegistry = clientFactory.GetEventRegistry();
        }

        #region 公开接口

        public void Start(LoggerVM viewModel)
        {
            if (!this.initOnce)
            {
                this.loggerEvent = new ManualResetEvent(false);
                this.loggerList = new List<LoggerVM>();
                this.loggerListCopy = new List<LoggerVM>();
                this.listChanged = false;
                this.listLock = new object();

                this.writeThread = new Thread(this.WriteThreadProc);
                this.writeThread.IsBackground = true;
                this.writeThread.Start();

                this.initOnce = true;
            }

            lock (this.listLock)
            {
                this.loggerList.Add(viewModel);
                this.listChanged = true;
            }

            viewModel.Status = LoggerStatus.Start;
            this.loggerEvent.Set();

            this.eventRegistry.SubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, viewModel.ClientTab, viewModel);
        }

        public void Stop(IClientTab shellTab)
        {
            LoggerVM viewModel = this.loggerList.FirstOrDefault(v => v.ClientTab == shellTab);
            if (viewModel == null)
            {
                return;
            }

            lock (this.listLock)
            {
                this.loggerList.Remove(viewModel);
                this.listChanged = true;
            }

            viewModel.Status = LoggerStatus.Stop;

            this.eventRegistry.UnsubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, shellTab);
        }

        #endregion

        #region 事件处理器

        private void OnShellRendered(TabEventArgs e, object userData)
        {
            TabEventShellRendered shellRendered = e as TabEventShellRendered;
            LoggerVM lstate = userData as LoggerVM;
            List<VTHistoryLine> pendingLines = lstate.PendingLines;

            lock (pendingLines)
            {
                pendingLines.AddRange(shellRendered.NewLines);
            }
        }

        private void WriteThreadProc()
        {
            log4netLogger.InfoFormat("写日志线程启动成功");

            StringBuilder builder = new StringBuilder();

            while (true)
            {
                if (this.loggerList.Count == 0)
                {
                    log4netLogger.InfoFormat("休眠写日志线程");
                    this.loggerEvent.Reset();
                    this.loggerEvent.WaitOne();
                    log4netLogger.InfoFormat("唤醒写日志线程");
                }

                if (this.listChanged)
                {
                    lock (this.listLock)
                    {
                        this.loggerListCopy.Clear();
                        this.loggerListCopy.AddRange(this.loggerList);
                        this.listChanged = false;
                    }
                }

                foreach (LoggerVM vtLogger in this.loggerListCopy)
                {
                    List<VTHistoryLine> pendingLines = vtLogger.PendingLines;
                    if (pendingLines.Count == 0)
                    {
                        continue;
                    }

                    List<VTHistoryLine> toWrite = null;

                    lock (pendingLines)
                    {
                        toWrite = pendingLines.ToList();
                        pendingLines.Clear();
                    }

                    foreach (VTHistoryLine historyLine in toWrite)
                    {
                        string text = VTDocUtils.CreatePlainText(historyLine.Characters);
                        builder.AppendLine(text);
                    }

                    string filePath = vtLogger.FilePath;

                    try
                    {
                        File.AppendAllText(filePath, builder.ToString());
                    }
                    catch (Exception ex)
                    {
                        log4netLogger.ErrorFormat("写入日志文件异常, {0}, {1}", filePath, ex);
                    }

                    builder.Clear();
                }

                Thread.Sleep(WriteInterval);
            }
        }

        #endregion
    }
}
