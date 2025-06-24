using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Session;
using ModengTerm.UserControls.TerminalUserControls;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Record
{
    /// <summary>
    /// OpenRecordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OpenRecordWindow : MdWindow
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("OpenRecordWindow");

        #endregion

        #region 实例变量

        private bool isPlaying;
        private BindableCollection<Playback> playbacks;
        private IClientTab tab;
        private StorageService storage;

        private VideoTerminal videoTerminal;
        private Task playbackTask;
        private AutoResetEvent playbackEvent;
        private PlaybackStatusEnum playbackStatus;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public OpenRecordWindow(IClientTab tab)
        {
            InitializeComponent();

            this.InitializeWindow(tab);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(IClientTab tab)
        {
            this.tab = tab;
            ClientFactory facory = ClientFactory.GetFactory();
            this.storage = facory.GetStorageService();

            List<Playback> playbacks = this.storage.GetObjects<Playback>(this.tab.ID.ToString());
            this.playbacks = new BindableCollection<Playback>();
            this.playbacks.AddRange(playbacks);
            ComboBoxPlaybackList.ItemsSource = this.playbacks;
            ComboBoxPlaybackList.SelectedIndex = 0;
        }

        private void OpenRecord(IClientTab tab, Playback playback)
        {
            MainWindowVM mainWindowVM = MainWindowVM.GetInstance();
            ShellSessionVM shellSession = mainWindowVM.SessionList.FirstOrDefault(v => v.ID == tab.ID) as ShellSessionVM;
            XTermSession session = shellSession.Session;

            this.playbackEvent = new AutoResetEvent(false);

            string background = session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);

            double padding = session.GetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING);
            double width = DocumentMain.ActualWidth;
            double height = DocumentMain.ActualHeight;

            DocumentAlternate.Padding = new Thickness(padding);
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;
            DocumentAlternate.Visibility = Visibility.Collapsed;
            DocumentMain.Padding = new Thickness(padding);
            DocumentMain.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;

            VTOptions options = new VTOptions()
            {
                Session = session,
                AlternateDocument = DocumentAlternate,
                MainDocument = DocumentMain,
                Width = width,
                Height = height,
                SessionTransport = new SessionTransport()
            };

            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.RequestChangeVisible += this.VideoTerminal_RequestChangeVisible;
            this.videoTerminal.Initialize(options);

            this.playbackTask = Task.Factory.StartNew(this.PlaybackThreadProc, playback);
        }

        private void CloseRecord()
        {
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown -= DrawArea_PreviewMouseRightButtonDown;
            DocumentMain.DrawArea.PreviewMouseRightButtonDown -= DrawArea_PreviewMouseRightButtonDown;

            this.playbackStatus = PlaybackStatusEnum.Stopped;
            this.playbackEvent.Set();
            this.playbackEvent.Close();
            //Task.WaitAll(this.playbackTask); // Wait可能会导致死锁
            this.playbackTask = null;

            this.videoTerminal.RequestChangeVisible -= this.VideoTerminal_RequestChangeVisible;
            this.videoTerminal.Release();
        }

        #endregion

        #region 事件处理器

        private void VideoTerminal_RequestChangeVisible(IVideoTerminal arg1, VTDocumentTypes type, bool visible)
        {
            VTDocument document = type == VTDocumentTypes.AlternateDocument ? this.videoTerminal.AlternateDocument : this.videoTerminal.MainDocument;
            TerminalControl terminalControl = type == VTDocumentTypes.AlternateDocument ? DocumentAlternate : DocumentMain;

            terminalControl.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (visible)
            {
                VTCursorTimer.Context.SetCursor(document.Cursor);
            }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (this.isPlaying)
            {
                this.CloseRecord();
                this.isPlaying = false;
            }

            Playback playback = ComboBoxPlaybackList.SelectedItem as Playback;
            if (playback == null)
            {
                MTMessageBox.Info("请选择要回放的文件");
                return;
            }

            if (!System.IO.File.Exists(playback.FullPath))
            {
                MTMessageBox.Info("回放文件不存在");
                return;
            }

            this.isPlaying = true;

            this.OpenRecord(this.tab, playback);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Playback playback = ComboBoxPlaybackList.SelectedItem as Playback;
            if (playback == null)
            {
                return;
            }

            if (this.isPlaying) 
            {
                MTMessageBox.Info("当前回放正在播放中, 无法删除, 请先停止当前回放");
                return;
            }

            if (!MTMessageBox.Confirm("确定要删除{0}吗?", playback.Name))
            {
                return;
            }

            int code = this.storage.DeleteObject(playback.Id);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Info("删除失败, {0}", code);
                return;
            }

            this.playbacks.Remove(playback);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.isPlaying) 
            {
                this.CloseRecord();

                this.isPlaying = false;
            }
        }

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
    }
}
