using ModengTerm.Base;
using ModengTerm.ServiceAgents.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Rendering;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    public class PlaybackVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PlaybackVM");

        #endregion

        #region 实例变量

        private Task playbackTask;
        private AutoResetEvent playbackEvent;
        private PlaybackStatusEnum playbackStatus;
        private PlaybackFile playbackFile;
        private PlaybackStream playbackStream;
        private VideoTerminal videoTerminal;

        #endregion

        #region 属性

        /// <summary>
        /// 终端渲染控件
        /// </summary>
        public DependencyObject Content { get; set; }
        
        #endregion

        #region 公开接口

        /// <summary>
        /// 打开回放
        /// </summary>
        /// <param name="file">要打开的回放文件</param>
        public int Open(PlaybackFile file)
        {
            this.playbackFile = file;

            this.playbackStream = new PlaybackStream();
            int code = this.playbackStream.OpenRead(this.playbackFile);
            if (code != ResponseCode.SUCCESS)
            {
                return code;
            }

            SessionTransport transport = new SessionTransport();

            VTOptions options = new VTOptions()
            {
                Session = this.playbackFile.Session,
                WindowHost = this.Content as IDrawingTerminal,
                SessionTransport = transport
            };
            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.Initialize(options);

            this.playbackStatus = PlaybackStatusEnum.Playing;
            this.playbackEvent = new AutoResetEvent(false);
            this.playbackTask = Task.Factory.StartNew(this.PlaybackThreadProc);

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 关闭回放
        /// </summary>
        public void Close()
        {
            this.playbackStatus = PlaybackStatusEnum.Idle;
            this.playbackEvent.Set();
            Task.WaitAll(this.playbackTask);
            this.playbackStream.Close();
            this.playbackEvent.Close();
            this.playbackEvent.Dispose();

            this.videoTerminal.Release();
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 回放
        /// </summary>
        private void PlaybackThreadProc()
        {
            long prevTimestamp = 0;

            while (this.playbackStatus == PlaybackStatusEnum.Playing)
            {
                if (this.playbackStream.EndOfFile)
                {
                    logger.InfoFormat("回放文件播放结束");
                    break;
                }

                try
                {
                    PlaybackFrame nextFrame = this.playbackStream.GetNextFrame();

                    if (nextFrame == null)
                    {
                        // 此时说明读取文件有异常
                        logger.ErrorFormat("读取回放文件下一帧失败, 结束回放");
                        break;
                    }

                    if (prevTimestamp > 0)
                    {
                        long interval = nextFrame.Timestamp - prevTimestamp;
                        this.playbackEvent.WaitOne((int)interval / 10000);
                    }

                    // 播放一帧
                    this.videoTerminal.ProcessData(nextFrame.Data, nextFrame.Data.Length);

                    prevTimestamp = nextFrame.Timestamp;
                }
                catch (Exception ex)
                {
                    logger.Error("回放异常", ex);
                }
            }

            logger.InfoFormat("回放线程运行结束, 关闭回放文件");

            this.playbackStream.Close();
            this.playbackStatus = PlaybackStatusEnum.Idle;
        }

        #endregion
    }
}
