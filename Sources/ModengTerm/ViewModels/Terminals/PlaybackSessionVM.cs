using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document.Drawing;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Parsing;
using ModengTerm.Terminal.Session;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    public class PlaybackOptions 
    {
        /// <summary>
        /// 存储回放信息
        /// </summary>
        public Playback Playback { get; set; }

        /// <summary>
        /// 回放关联的会话
        /// </summary>
        public XTermSession Session { get; set; }

        public IDocument AlternateDocument { get; set; }

        public IDocument MainDocument { get; set; }

        /// <summary>
        /// 终端的宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 终端的高度
        /// </summary>
        public double Height { get; set; }
    }

    public class PlaybackSessionVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PlaybackVM");

        #endregion

        #region 实例变量

        private Task playbackTask;
        private PlaybackStatusEnum playbackStatus;
        private Playback playback;
        private AutoResetEvent playbackEvent;

        private PlaybackOptions options;
        private VideoTerminal videoTerminal;
        private SessionTransport sessionTransport;

        #endregion

        #region 属性

        public IVideoTerminal VideoTerminal { get { return this.videoTerminal; } }

        #endregion

        #region 公开接口

        /// <summary>
        /// 打开回放
        /// </summary>
        /// <param name="file">要打开的回放文件</param>
        public int Open(PlaybackOptions options)
        {
            this.options = options;
            this.playback = options.Playback;
            this.playbackEvent = new AutoResetEvent(false);

            #region 初始化终端

            VTOptions vtOptions = new VTOptions()
            {
                AlternateDocument = options.AlternateDocument,
                MainDocument = options.MainDocument,
                Session = options.Session,
                Width = options.Width,
                Height = options.Height,
                SessionTransport = new SessionTransport()
            };
            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.Initialize(vtOptions);

            #endregion

            this.playbackTask = Task.Factory.StartNew(this.PlaybackThreadProc);

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 关闭回放
        /// </summary>
        public void Close()
        {
            this.playbackStatus = PlaybackStatusEnum.Stopped;
            this.playbackEvent.Set();
            this.playbackEvent.Close();
            //Task.WaitAll(this.playbackTask); // Wait可能会导致死锁
            this.playbackTask = null;
            this.videoTerminal.Release();
        }

        #endregion

        #region 事件处理器

        private void SessionTransport_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.videoTerminal.ProcessData(bytes, size);
                }
                catch (Exception ex)
                {
                    logger.Error("回放Render异常", ex);
                }
            });
        }

        private void SessionTransport_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("回放状态发生改变, {0}", status);

            try
            {
                switch (status)
                {
                    case SessionStatusEnum.Connected:
                        {
                            break;
                        }

                    case SessionStatusEnum.Connecting:
                        {
                            break;
                        }

                    case SessionStatusEnum.ConnectError:
                        {
                            break;
                        }

                    case SessionStatusEnum.Disconnected:
                        {
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                logger.Error("处理回放状态异常", ex);
            }
        }

        /// <summary>
        /// 回放
        /// </summary>
        private void PlaybackThreadProc()
        {
            long prevTimestamp = 0;

            PlaybackStream playbackStream = new PlaybackStream();
            int code = playbackStream.OpenRead(this.playback);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开录像文件失败, {0}", code);
                return;
            }

            this.playbackStatus = PlaybackStatusEnum.Playing;

            while (this.playbackStatus == PlaybackStatusEnum.Playing)
            {
                if (playbackStream.EndOfFile)
                {
                    logger.InfoFormat("回放文件播放结束");
                    break;
                }

                try
                {
                    PlaybackFrame nextFrame = playbackStream.GetNextFrame();

                    if (nextFrame == null)
                    {
                        // 此时说明读取文件有异常
                        logger.ErrorFormat("读取回放文件下一帧失败, 结束回放");
                        break;
                    }

                    if (prevTimestamp > 0)
                    {
                        long interval = nextFrame.Timestamp - prevTimestamp;
                        playbackEvent.WaitOne((int)interval / 10000);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // 播放一帧
                        if (this.playbackStatus == PlaybackStatusEnum.Playing)
                        {
                            this.videoTerminal.ProcessData(nextFrame.Data, nextFrame.Data.Length);
                        }
                    });

                    prevTimestamp = nextFrame.Timestamp;
                }
                catch (Exception ex)
                {
                    logger.Error("回放异常", ex);
                }
            }

            logger.InfoFormat("回放线程运行结束, 关闭回放文件");

            playbackStream.Close();

            this.playbackStatus = PlaybackStatusEnum.Stopped;
        }

        #endregion
    }
}
