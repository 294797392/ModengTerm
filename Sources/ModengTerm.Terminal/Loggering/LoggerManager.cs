using DotNEToolkit.Modular;
using ModengTerm.Base;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XTerminal.Base;

namespace ModengTerm.Terminal.Loggering
{
    public class LoggerManager
    {
        #region 类变量

        private static log4net.ILog log4netLogger = log4net.LogManager.GetLogger("LoggerManager");

        /// <summary>
        /// 写入日志文件的间隔时间，单位是毫秒
        /// </summary>
        private const int WriteInterval = 3000;

        #endregion

        #region 实例变量

        private Thread writeThread;
        private ManualResetEvent loggerEvent;
        private List<VTLogger> loggerList;
        private List<VTLogger> loggerListCopy;
        private bool listChanged;
        private object listLock;

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        public int Initialize()
        {
            this.loggerEvent = new ManualResetEvent(false);
            this.loggerList = new List<VTLogger>();
            this.loggerListCopy = new List<VTLogger>();
            this.listChanged = false;
            this.listLock = new object();

            this.writeThread = new Thread(this.WriteThreadProc);
            this.writeThread.IsBackground = true;
            this.writeThread.Start();

            return ResponseCode.SUCCESS;
        }

        public void Release()
        {
        }

        public void Start(IVideoTerminal vt, LoggerOptions options)
        {
            VTLogger logger = vt.Logger;
            if (logger != null)
            {
                return;
            }

            LoggerFilter filter = LoggerFilterFactory.Create(options.FilterType);
            filter.FilterText = options.FilterText;

            logger = new VTLogger()
            {
                Filter = filter,
                Builder = new StringBuilder(),
                CreateLine = VTUtils.GetCreateLineDelegate(options.FileType),
                FilePath = options.FilePath,
                FileType = options.FileType,
                IsPaused = false
            };

            vt.Logger = logger;
            vt.LinePrinted += this.VideoTerminal_LinePrinted;

            lock (this.listLock)
            {
                this.loggerList.Add(logger);
                this.listChanged = true;
            }

            this.loggerEvent.Set();
        }

        public void Stop(IVideoTerminal vt)
        {
            VTLogger logger = vt.Logger;
            if (logger == null)
            {
                return;
            }

            vt.Logger = null;
            vt.LinePrinted -= this.VideoTerminal_LinePrinted;
            logger.Dispose();

            lock (this.listLock)
            {
                this.loggerList.Remove(logger);
                this.listChanged = true;
            }
        }

        public void Pause(IVideoTerminal vt)
        {
            VTLogger logger = vt.Logger as VTLogger;
            if (logger == null)
            {
                return;
            }

            logger.IsPaused = true;
        }

        public void Resume(IVideoTerminal vt)
        {
            VTLogger logger = vt.Logger as VTLogger;
            if (logger == null)
            {
                return;
            }

            logger.IsPaused = false;
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当行被打印的时候触发
        /// </summary>
        /// <param name="vt">触发该事件的终端</param>
        /// <param name="historyLine">被打印出来的行数据</param>
        private void VideoTerminal_LinePrinted(IVideoTerminal vt, VTHistoryLine historyLine)
        {
            VTLogger logger = vt.Logger;
            if (logger.IsPaused)
            {
                // 如果该日志记录器被暂停，那么什么都不记录
                return;
            }

            lock (logger.Builder)
            {
                // TODO：
                logger.CreateLine(historyLine.Characters, logger.Builder, 0, historyLine.Characters.Count, false);
            }
        }

        private void WriteThreadProc()
        {
            log4netLogger.InfoFormat("写日志线程启动成功");

            while (true)
            {
                if (this.loggerList.Count == 0)
                {
                    this.loggerEvent.Reset();
                    this.loggerEvent.WaitOne();
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

                foreach (VTLogger vtLogger in this.loggerListCopy)
                {
                    StringBuilder builder = vtLogger.Builder;
                    if (builder.Length == 0)
                    {
                        continue;
                    }

                    string content = string.Empty;
                    lock (vtLogger.Builder)
                    {
                        content = builder.ToString();
                        builder.Clear();
                    }
                    string filePath = vtLogger.FilePath;

                    try
                    {
                        File.AppendAllText(filePath, content);
                    }
                    catch (Exception ex)
                    {
                        log4netLogger.ErrorFormat("写入日志文件异常, {0}, {1}", filePath, ex);
                    }
                }

                Thread.Sleep(WriteInterval);
            }
        }

        #endregion
    }
}
