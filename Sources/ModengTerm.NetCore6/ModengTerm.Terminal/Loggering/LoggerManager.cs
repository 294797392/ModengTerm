using DotNEToolkit.Modular;
using ModengTerm.Base;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Document;

namespace ModengTerm.Terminal.Loggering
{
    public class LoggerManager //: AppModule<MTermManifest>
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("LoggerManager");

        #endregion

        #region 实例变量

        private Thread writeThread;
        private List<LoggerContext> loggerList;
        private List<LoggerContext> loggerListCopy;
        private bool changed;
        private object listLock;
        private ManualResetEvent loggerEvent;
        private bool allPaused;

        #endregion

        #region AppModule

        protected int OnInitialize()
        {
            this.loggerEvent = new ManualResetEvent(false);
            this.listLock = new object();
            this.loggerList = new List<LoggerContext>();
            this.loggerListCopy = new List<LoggerContext>();

            this.writeThread = new Thread(this.WriteThreadProc);
            this.writeThread.IsBackground = true;
            this.writeThread.Start();

            return ResponseCode.SUCCESS;
        }

        protected void OnRelease()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        public void Start(VideoTerminal vt, LoggerOptions options)
        {
            LoggerFilter filter = LoggerFilterFactory.Create(options.FilterType);
            filter.FilterText = options.FilterText;

            LoggerContext loggerContext = new LoggerContext()
            {
                Options = options,
                VideoTerminal = vt,
                Filter = filter,
                NextLine = 0,
                Builder = new StringBuilder()
            };

            lock (this.listLock)
            {
                this.changed = true;
                this.loggerList.Add(loggerContext);
            }

            this.loggerEvent.Set();
        }

        public void Stop(VideoTerminal vt)
        {
            LoggerContext loggerContext = this.loggerList.FirstOrDefault(v => v.VideoTerminal == vt);
            if (loggerContext != null)
            {
                lock (this.listLock)
                {
                    loggerContext.Dispose();
                    this.loggerList.Remove(loggerContext);
                    this.changed = true;
                }
            }
        }

        public void Pause(VideoTerminal vt)
        {
            LoggerContext loggerContext = this.loggerList.FirstOrDefault(v => v.VideoTerminal == vt);
            if (loggerContext == null)
            {
                logger.WarnFormat("LoggerPause失败, 未找到Logger");
                return;
            }

            loggerContext.IsPaused = true;

            this.allPaused = this.loggerList.All(v => v.IsPaused);
        }

        public void Resume(VideoTerminal vt)
        {
            LoggerContext loggerContext = this.loggerList.FirstOrDefault(v => v.VideoTerminal == vt);
            if (loggerContext == null)
            {
                logger.WarnFormat("LoggerResume失败, 未找到Logger");
                return;
            }

            loggerContext.IsPaused = false;

            this.allPaused = false;

            this.loggerEvent.Set();
        }

        #endregion

        #region 事件处理器

        private void WriteThreadProc()
        {
            logger.InfoFormat("日志记录线程启动成功");

            while (true)
            {
                //if (this.loggerListCopy.Count == 0 || this.allPaused)
                //{
                //    this.loggerEvent.Reset();
                //    this.loggerEvent.WaitOne();
                //}

                //if (this.changed)
                //{
                //    lock (this.listLock)
                //    {
                //        this.loggerListCopy.Clear();
                //        this.loggerListCopy.AddRange(this.loggerList);
                //        this.changed = false;
                //    }
                //}

                //foreach (LoggerContext context in this.loggerListCopy)
                //{
                //    VideoTerminal vt = context.VideoTerminal;

                //    // 最后一行不记录，只记录到倒数第二行
                //    // 因为最后一行的数据有可能会变化

                //    try
                //    {
                //        if (vt.lastHistoryLine.PhysicsRow == context.NextLine)
                //        {
                //            continue;
                //        }

                //        VTHistoryLine startLine = vt.historyLines[context.NextLine];
                //        VTHistoryLine endLine = vt.lastHistoryLine.PreviousLine;

                //        // 先对日志进行过滤
                //        VTUtils.BuildDocument(startLine, endLine, 0, endLine.Characters.Count - 1, context.Builder, context.FileType, context.Filter);

                //        string text = context.Builder.ToString();

                //        try
                //        {
                //            File.AppendAllText(context.FilePath, text);
                //        }
                //        catch (Exception ex)
                //        {
                //            logger.Error("保存日志文件异常", ex);
                //        }

                //        context.Builder.Clear();

                //        context.NextLine = vt.lastHistoryLine.PhysicsRow;
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.Error("Logger运行异常", ex);
                //    }
                //}

                Thread.Sleep(3000);
            }
        }

        #endregion
    }
}
