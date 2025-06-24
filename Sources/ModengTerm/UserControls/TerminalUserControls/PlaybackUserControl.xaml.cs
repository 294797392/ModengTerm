using log4net.Repository.Hierarchy;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.OfficialAddons.Record;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Session;
using ModengTerm.UserControls.TerminalUserControls;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModengTerm.UserControls.Terminals
{
    /// <summary>
    /// PlaybackUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlaybackUserControl : UserControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PlaybackUserControl");

        #endregion

        #region 实例变量

        private XTermSession session;
        private VideoTerminal videoTerminal;
        private Task playbackTask;
        private AutoResetEvent playbackEvent;
        private PlaybackStatusEnum playbackStatus;

        #endregion

        #region 构造方法

        public PlaybackUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl() 
        {
            this.playbackEvent = new AutoResetEvent(false);
        }

        #endregion

        #region 事件处理器

        private void DrawArea_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 右键直接复制选中内容
            VTParagraph paragraph = this.videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            if (paragraph.IsEmpty)
            {
                return;
            }

            // 把数据设置到Windows剪贴板里
            System.Windows.Clipboard.SetText(paragraph.Content);

            this.videoTerminal.UnSelectAll();
        }

        /// <summary>
        /// 回放线程
        /// </summary>
        private void PlaybackThreadProc(object state)
        {
            Playback playback = state as Playback;

            long prevTimestamp = 0;

            PlaybackStream playbackStream = new PlaybackStream();
            int code = playbackStream.OpenRead(playback.FullPath);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开录像文件失败, {0}", code);
                return;
            }

            playbackStatus = PlaybackStatusEnum.Playing;

            while (playbackStatus == PlaybackStatusEnum.Playing)
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

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        // 播放一帧
                        if (playbackStatus == PlaybackStatusEnum.Playing)
                        {
                            videoTerminal.ProcessRead(nextFrame.Data, nextFrame.Data.Length);
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

            playbackStatus = PlaybackStatusEnum.Stopped;
        }

        #endregion

        #region 公开接口

        public void Open(Playback playback)
        {
            this.session = playback.Session;

            string background = session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);

            double padding = session.GetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING);
            double width = DocumentMain.ActualWidth;
            double height = DocumentMain.ActualHeight;

            DocumentAlternate.Padding = new Thickness(padding);
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;
            DocumentMain.Padding = new Thickness(padding);
            DocumentMain.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;

            VTOptions options = new VTOptions()
            {
                Session = playback.Session,
                AlternateDocument = DocumentAlternate,
                MainDocument = DocumentMain,
                Width = width,
                Height = height,
                SessionTransport = new SessionTransport()
            };

            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.Initialize(options);

            this.playbackTask = Task.Factory.StartNew(this.PlaybackThreadProc, playback);
        }

        public void Close()
        {
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown -= DrawArea_PreviewMouseRightButtonDown;
            DocumentMain.DrawArea.PreviewMouseRightButtonDown -= DrawArea_PreviewMouseRightButtonDown;

            this.playbackStatus = PlaybackStatusEnum.Stopped;
            this.playbackEvent.Set();
            this.playbackEvent.Close();
            //Task.WaitAll(this.playbackTask); // Wait可能会导致死锁
            this.playbackTask = null;

            this.videoTerminal.Release();
        }

        #endregion
    }
}
