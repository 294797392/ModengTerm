using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
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
using System.Windows.Controls.Primitives;
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

        private object playLock = new object();
        private Playback playback;
        private VideoTerminal videoTerminal;
        private Task playTask;
        private AutoResetEvent playEvent;

        private int scrollDelta;

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

            this.playEvent = new AutoResetEvent(false);

            // 确保DrawArea不为空
            Dispatcher.BeginInvoke(this.InitializeVideoTerminal, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void InitializeVideoTerminal()
        {
            MainWindowVM mainWindowVM = MainWindowVM.GetInstance();
            ShellSessionVM shellSession = mainWindowVM.SessionList.FirstOrDefault(v => v.ID == tab.ID) as ShellSessionVM;
            XTermSession session = shellSession.Session;

            this.scrollDelta = session.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA, OptionDefaultValues.MOUSE_SCROLL_DELTA);

            string background = session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);
            double padding = session.GetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING);
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;
            DocumentAlternate.Scrollbar.MouseMove += this.Scrollbar_MouseMove;
            DocumentAlternate.Padding = new Thickness(padding);
            DocumentAlternate.Visibility = Visibility.Collapsed;
            DocumentMain.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;
            DocumentMain.Scrollbar.MouseMove += this.Scrollbar_MouseMove;
            DocumentMain.Padding = new Thickness(padding);

            VTOptions options = new VTOptions()
            {
                Session = session,
                AlternateDocument = DocumentAlternate,
                MainDocument = DocumentMain,
                Width = DocumentMain.ActualWidth,
                Height = DocumentMain.ActualHeight,
                SessionTransport = new SessionTransport()
            };

            VideoTerminal videoTerminal = new VideoTerminal();
            videoTerminal.RequestChangeVisible += this.VideoTerminal_RequestChangeVisible;
            videoTerminal.Initialize(options);

            this.videoTerminal = videoTerminal;
        }

        private void ReleaseVideoTerminal()
        {
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;
            DocumentAlternate.Scrollbar.MouseMove -= this.Scrollbar_MouseMove;
            DocumentMain.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;
            DocumentMain.Scrollbar.MouseMove -= this.Scrollbar_MouseMove;
            this.videoTerminal.RequestChangeVisible -= this.VideoTerminal_RequestChangeVisible;
            this.videoTerminal.Release();
        }

        private void OpenPlay()
        {
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

            this.playback = playback;
            this.isPlaying = true;

            this.playTask = Task.Factory.StartNew(this.PlaybackThreadProc);

            ButtonPlay.IsEnabled = true;
        }

        private void ClosePlay(Action closedCallback)
        {
            this.playEvent.Set();

            // 确保播放线程已退出
            Task.Factory.StartNew(() =>
            {
                // 在UI线程Wait会导致死锁
                Task.WaitAll(this.playTask);

                Dispatcher.Invoke(closedCallback);
            });
        }

        /// <summary>
        /// 让scrollbar的值变成整数
        /// </summary>
        /// <param name="scrollbar"></param>
        /// <returns></returns>
        private int GetScrollValue(ScrollBar scrollbar)
        {
            var newvalue = (int)Math.Round(scrollbar.Value, 0);
            if (newvalue > scrollbar.Maximum)
            {
                newvalue = (int)Math.Round(scrollbar.Maximum, 0);
            }
            return newvalue;
        }

        private Point GetPositionRelativeToTerminal()
        {
            VTDocument document = this.videoTerminal.ActiveDocument;
            TerminalControl terminalControl = document.GFactory as TerminalControl;
            return Mouse.GetPosition(terminalControl.DrawArea);
        }

        private void MouseWheelScroll(MouseWheelEventArgs e)
        {
            VTDocument document = this.videoTerminal.ActiveDocument;

            int oldScroll = document.Scrollbar.Value;
            int scrollMax = document.Scrollbar.Maximum;
            int newScroll = 0; // 最终要滚动到的值

            if (e.Delta > 0)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (document.ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (oldScroll < scrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    newScroll = document.Scrollbar.Minimum;
                }
                else
                {
                    newScroll = oldScroll - scrollDelta;
                }
            }
            else
            {
                // 向下滚动

                if (document.ScrollAtBottom)
                {
                    // 滚动到底直接返回
                    return;
                }

                // 剩余可以往下滚动的行数
                int remainScroll = scrollMax - oldScroll;

                if (remainScroll >= scrollDelta)
                {
                    newScroll = oldScroll + scrollDelta;
                }
                else
                {
                    // 直接滚动到底
                    newScroll = scrollMax;
                }
            }

            VTScrollData scrollData = document.ScrollTo(newScroll);
            if (scrollData == null)
            {
                return;
            }

            // 重新渲染
            document.RequestInvalidate();
        }

        #endregion

        #region 事件处理器

        private void GridTerminal_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (GridTerminal.IsMouseCaptured)
                {
                    GridTerminal.CaptureMouse();
                }

                Point position = this.GetPositionRelativeToTerminal();
                VTDocument document = this.videoTerminal.ActiveDocument;
                document.PerformSelection(position.X, position.Y);
            }
        }

        private void GridTerminal_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.MouseWheelScroll(e);
        }

        private void GridTerminal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                VTDocument document = this.videoTerminal.ActiveDocument;
                if (!document.Selection.IsEmpty)
                {
                    document.Selection.Clear();
                    document.Selection.RequestInvalidate();
                }

                if (e.ClickCount == 1)
                {
                }
                else
                {
                    // 双击就是选中单词
                    // 三击就是选中整行内容

                    Point position = this.GetPositionRelativeToTerminal();
                    double x = position.X;
                    double y = position.Y;
                    int startIndex = 0, count = 0, logicalRow = 0;

                    VTextLine textLine = HitTestHelper.HitTestVTextLine(document, y, out logicalRow);
                    if (textLine == null)
                    {
                        return;
                    }

                    switch (e.ClickCount)
                    {
                        case 2:
                            {
                                // 选中单词
                                int characterIndex;
                                int columnIndex;
                                VTextRange characterRange;
                                HitTestHelper.HitTestVTCharacter(textLine, x, out characterIndex, out characterRange, out columnIndex);
                                if (characterIndex == -1)
                                {
                                    return;
                                }
                                VTDocUtils.GetSegement(textLine.Characters, characterIndex, out startIndex, out count);
                                document.Selection.SelectRange(textLine, logicalRow, startIndex, count);
                                break;
                            }

                        case 3:
                            {
                                // 选中一整行
                                document.Selection.SelectRow(textLine, logicalRow);
                                break;
                            }

                        default:
                            break;
                    }
                }
            }
            else
            {
                VTParagraph paragraph = this.videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
                if (paragraph.IsEmpty)
                {
                    return;
                }

                // 把数据设置到Windows剪贴板里
                System.Windows.Clipboard.SetText(paragraph.Content);

                this.videoTerminal.UnSelectAll();
            }
        }

        private void GridTerminal_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (GridTerminal.IsMouseCaptured)
                {
                    GridTerminal.ReleaseMouseCapture();
                }
            }
        }


        private void VideoTerminal_RequestChangeVisible(IVideoTerminal arg1, VTDocumentTypes type, bool visible)
        {
            VTDocument document = type == VTDocumentTypes.AlternateDocument ? arg1.AlternateDocument : arg1.MainDocument;
            TerminalControl terminalControl = type == VTDocumentTypes.AlternateDocument ? DocumentAlternate : DocumentMain;

            terminalControl.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (visible)
            {
                VTCursorTimer.Context.SetCursor(document.Cursor);
            }
        }

        private void Scrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ScrollBar scrollbar = sender as ScrollBar;

                int newscroll = this.GetScrollValue(scrollbar);
                VTDocument document = this.videoTerminal.ActiveDocument;
                document.ScrollTo(newscroll);
                document.RequestInvalidate();

                e.Handled = true;
            }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            ButtonPlay.IsEnabled = false;

            if (this.isPlaying)
            {
                this.isPlaying = false;
                this.ClosePlay(this.OpenPlay);
            }
            else
            {
                this.OpenPlay();
            }
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
                this.isPlaying = false;

                this.ClosePlay(this.ReleaseVideoTerminal);
            }
        }

        private void DrawArea_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 右键直接复制选中内容
            VTParagraph paragraph = videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            if (paragraph.IsEmpty)
            {
                return;
            }

            // 把数据设置到Windows剪贴板里
            System.Windows.Clipboard.SetText(paragraph.Content);

            videoTerminal.UnSelectAll();
        }

        private void PlaybackThreadProc()
        {
            Playback playback = this.playback;

            long prevTimestamp = 0;

            PlaybackStream playbackStream = new PlaybackStream();
            int code = playbackStream.OpenRead(playback.FullPath);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开录像文件失败, {0}", code);
                return;
            }

            while (this.isPlaying)
            {
                if (playbackStream.EndOfFile)
                {
                    //logger.InfoFormat("回放文件播放结束");
                    break;
                }

                try
                {
                    PlaybackFrame nextFrame = playbackStream.GetNextFrame();

                    if (nextFrame == null)
                    {
                        // 此时说明读取文件有异常
                        logger.ErrorFormat("读取回放文件下一帧失败");
                        break;
                    }

                    if (prevTimestamp > 0)
                    {
                        long interval = nextFrame.Timestamp - prevTimestamp;
                        playEvent.WaitOne((int)interval / 10000);
                    }

                    // 播放一帧
                    if (this.isPlaying)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            videoTerminal.ProcessRead(nextFrame.Data, nextFrame.Data.Length);
                        });
                    }

                    prevTimestamp = nextFrame.Timestamp;
                }
                catch (Exception ex)
                {
                    logger.Error("回放异常", ex);
                }
            }

            playbackStream.Close();

            logger.InfoFormat("回放线程运行结束");
        }

        private void GridTerminal_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.videoTerminal != null)
            {
                this.videoTerminal.Resize(e.NewSize.ToVTSize());
            }
        }

        #endregion
    }
}
