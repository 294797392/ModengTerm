using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace ModengTerm.OfficialAddons.Logger
{
    public class LoggerAddon : AddonModule
    {
        #region 类变量

        private static log4net.ILog log4netLogger = log4net.LogManager.GetLogger("LoggerAddon");

        /// <summary>
        /// 写入日志文件的间隔时间，单位是毫秒
        /// </summary>
        private const int WriteInterval = 3000;

        private const string KEY_LOGGER = "logger";

        #endregion

        #region 实例变量

        private Thread writeThread;
        private ManualResetEvent loggerEvent;
        private bool initOnce = false;
        private List<LoggerContext> loggerList;
        private List<LoggerContext> loggerListCopy;
        private bool listChanged;
        private object listLock;

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("LoggerAddon.StartLogger", this.ExecuteStartLoggerCommand);
            this.RegisterCommand("LoggerAddon.StopLogger", this.ExecuteStopLoggerCommand);
        }

        protected override void OnDeactive()
        {
        }

        #endregion

        #region 实例方法

        private int Initialize()
        {
            this.loggerEvent = new ManualResetEvent(false);
            this.loggerList = new List<LoggerContext>();
            this.loggerListCopy = new List<LoggerContext>();
            this.listChanged = false;
            this.listLock = new object();

            this.writeThread = new Thread(this.WriteThreadProc);
            this.writeThread.IsBackground = true;
            this.writeThread.Start();

            return ResponseCode.SUCCESS;
        }

        private LoggerContext Start(LoggerOptionsVM options)
        {
            LoggerContext logger = new LoggerContext()
            {
                FilePath = options.FilePath,
            };

            lock (this.listLock)
            {
                this.loggerList.Add(logger);
                this.listChanged = true;
            }

            this.loggerEvent.Set();

            return logger;
        }

        private void Stop(IClientTab tab)
        {
            LoggerContext logger = tab.GetData<LoggerContext>(this, KEY_LOGGER);
            if (logger == null)
            {
                return;
            }

            lock (this.listLock)
            {
                this.loggerList.Remove(logger);
                this.listChanged = true;
            }

            tab.SetData(this, KEY_LOGGER, null);

            this.eventRegistry.UnsubscribeTabEvent(TabEvent.TAB_CLOSED, this.OnTabClosed, tab);
            this.eventRegistry.UnsubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, tab);
        }

        #endregion

        #region 事件处理器

        private void ExecuteStartLoggerCommand(CommandArgs e)
        {
            LoggerOptionsWindow window = new LoggerOptionsWindow(e.ActiveTab);
            window.Owner = Application.Current.MainWindow;
            if (!(bool)window.ShowDialog())
            {
                return;
            }

            if (!this.initOnce)
            {
                this.Initialize();
                this.initOnce = true;
            }

            IClientTab activeTab = e.ActiveTab;

            LoggerContext logger = this.Start(window.Options);
            activeTab.SetData(this, KEY_LOGGER, logger);

            this.eventRegistry.SubscribeTabEvent(TabEvent.TAB_CLOSED, this.OnTabClosed, activeTab);
            this.eventRegistry.SubscribeTabEvent(TabEvent.SHELL_RENDERED, this.OnShellRendered, activeTab);
        }

        private void ExecuteStopLoggerCommand(CommandArgs e)
        {
            IClientTab tab = e.ActiveTab;
            this.Stop(tab);
        }

        private void OnShellRendered(TabEventArgs e)
        {
            TabEventShellRendered shellRendered = e as TabEventShellRendered;
            LoggerContext logger = e.Sender.GetData<LoggerContext>(this, KEY_LOGGER);
            List<VTHistoryLine> pendingLines = logger.PendingLines;

            lock (pendingLines)
            {
                pendingLines.AddRange(shellRendered.NewLines);
            }
        }

        private void OnTabClosed(TabEventArgs e)
        {
            this.Stop(e.Sender);
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

                foreach (LoggerContext vtLogger in this.loggerListCopy)
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
