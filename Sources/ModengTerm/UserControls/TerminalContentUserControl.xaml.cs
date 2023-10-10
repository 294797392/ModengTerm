using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Rendering;
using ModengTerm.ServiceAgents;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Rendering;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;
using XTerminal.Enumerations;

namespace XTerminal.UserControls
{
    /// <summary>
    /// TerminalContentUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalContentUserControl : SessionContent, IDrawingWindow
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #endregion

        #region 实例变量

        private VideoTerminal videoTerminal;

        /// <summary>
        /// 提供剪贴板功能
        /// </summary>
        private VTClipboard clipboard;

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        private ServiceAgent serviceAgent;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public TerminalContentUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
#if !DEBUG
            ToggleButtonLoggerSetting.Visibility = Visibility.Collapsed;
#endif

            this.serviceAgent = MTermApp.Context.ServiceAgent;
            this.Background = Brushes.Transparent;
        }
        
        private LogFileTypeEnum FilterIndex2FileType(int filterIndex)
        {
            switch (filterIndex)
            {
                case 1: return LogFileTypeEnum.PlainText;
                case 2: return LogFileTypeEnum.HTML;

                default:
                    throw new NotImplementedException();
            }
        }
        
        private void SaveToFile(ParagraphTypeEnum saveMode)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            saveFileDialog.FileName = string.Format("{0}_{1}", this.videoTerminal.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if ((bool)saveFileDialog.ShowDialog())
            {
                LogFileTypeEnum fileType = this.FilterIndex2FileType(saveFileDialog.FilterIndex);

                try
                {
                    VTParagraph paragraph = this.videoTerminal.CreateParagraph(saveMode, fileType);
                    File.WriteAllText(saveFileDialog.FileName, paragraph.Content);
                }
                catch (Exception ex)
                {
                    logger.Error("保存日志异常", ex);
                    MessageBoxUtils.Error("保存失败");
                }
            }
        }

        #endregion

        #region 事件处理器

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (this.videoTerminal == null)
            {
                return;
            }

            this.videoTerminal.OnMouseWheel(this, e.Delta > 0);
        }

        /// <summary>
        /// 重写了这个事件后，就会触发鼠标相关的事件
        /// </summary>
        /// 参考AvalonEdit
        /// <param name="hitTestParameters"></param>
        /// <returns></returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }


        private void SliderScrolbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.videoTerminal.ScrollTo((int)e.NewValue);
        }



        private void GridCanvasList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.videoTerminal == null)
            {
                return;
            }

            Point p = e.GetPosition(GridCanvasList);

            GridCanvasList.CaptureMouse();
            this.videoTerminal.OnMouseDown(this, new VTPoint(p.X, p.Y), e.ClickCount);
        }

        private void GridCanvasList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.videoTerminal == null)
            {
                return;
            }

            if (GridCanvasList.IsMouseCaptured)
            {
                Point p = e.GetPosition(GridCanvasList);
                this.videoTerminal.OnMouseMove(this, new VTPoint(p.X, p.Y));
            }
        }

        private void GridCanvasList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.videoTerminal == null)
            {
                return;
            }

            Point p = e.GetPosition(GridCanvasList);
            this.videoTerminal.OnMouseUp(this, new VTPoint(p.X, p.Y));
            GridCanvasList.ReleaseMouseCapture();
        }

        private void GridCanvasList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.videoTerminal == null)
            {
                return;
            }

            this.videoTerminal.OnSizeChanged(this, this.GetDisplayRect());
        }



        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            VTParagraph paragraph = this.videoTerminal.GetSelectedParagraph();
            if (paragraph.IsEmpty)
            {
                return;
            }

            this.clipboard.SetData(paragraph);

            // 把数据设置到Windows剪贴板里
            System.Windows.Clipboard.SetText(paragraph.Content);
        }

        private void MenuItemPaste_Click(object sender, RoutedEventArgs e)
        {
            string text = System.Windows.Clipboard.GetText();
            this.videoTerminal.SendInput(text);
        }

        private void MenuItemSelectAll_Click(object sender, RoutedEventArgs e)
        {
            this.videoTerminal.SelectAll();
        }

        #region 保存日志

        private void MenuItemSaveDocument_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ParagraphTypeEnum.SaveDocument);
        }

        private void MenuItemSaveSelected_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ParagraphTypeEnum.SaveSelected);
        }

        private void MenuItemSaveAll_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ParagraphTypeEnum.SaveAll);
        }

        #endregion

        #region 日志记录

        private void MenuItemStartLogger_Click(object sender, RoutedEventArgs e)
        {
            LoggerOptionsWindow window = new LoggerOptionsWindow(this.videoTerminal);
            window.Owner = Window.GetWindow(this);
            if ((bool)window.ShowDialog())
            {
                MTermApp.Context.LoggerManager.Start(this.videoTerminal, window.Options);
            }
        }

        private void MenuItemStopLogger_Click(object sender, RoutedEventArgs e)
        {
            MTermApp.Context.LoggerManager.Stop(this.videoTerminal);
        }

        private void MenuItemPauseLogger_Click(object sender, RoutedEventArgs e)
        {
            MTermApp.Context.LoggerManager.Pause(this.videoTerminal);
        }

        private void MenuItemResumeLogger_Click(object sender, RoutedEventArgs e)
        {
            MTermApp.Context.LoggerManager.Resume(this.videoTerminal);
        }

        private void MenuItemOpenLoggerDirectory_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemFind_Click(object sender, RoutedEventArgs e)
        {
            FindVM vtFind = new FindVM(this.videoTerminal);
            FindWindow findWindow = new FindWindow(vtFind);
            findWindow.Owner = Window.GetWindow(this);
            findWindow.Show();
        }

        /// <summary>
        /// 剪贴板历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemClipboardHistory_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;

            ClipboardParagraphSource clipboardParagraphSource = new ClipboardParagraphSource(this.clipboard);
            clipboardParagraphSource.Session = this.Session;

            ClipboardVM clipboardVM = new ClipboardVM(clipboardParagraphSource, this.videoTerminal);
            clipboardVM.SendToAllTerminalDlg = mainWindow.SendToAllTerminal;

            ParagraphsWindow paragraphsWindow = new ParagraphsWindow(clipboardVM);
            paragraphsWindow.Title = "剪贴板历史";
            paragraphsWindow.Owner = mainWindow;
            paragraphsWindow.Show();
        }

        /// <summary>
        /// 添加到收藏夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            VTParagraph paragraph = this.videoTerminal.GetSelectedParagraph();
            if (paragraph.IsEmpty)
            {
                return;
            }

            Favorites favorites = new Favorites() 
            {
                ID = Guid.NewGuid().ToString(),
                Background = this.Session.GetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR),
                Foreground = this.Session.GetOption<string>(OptionKeyEnum.SSH_THEME_FORE_COLOR),
                SessionID = this.Session.ID,
                StartCharacterIndex = paragraph.StartCharacterIndex,
                EndCharacterIndex = paragraph.EndCharacterIndex,
                FontSize = this.Session.GetOption<double>(OptionKeyEnum.SSH_THEME_FONT_SIZE),
                CharacterList = paragraph.CharacterList,
                CreationTime = paragraph.CreationTime,
                FontFamily = this.Session.GetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY),
            };

            int code = this.serviceAgent.AddFavorites(favorites);
            if(code != ResponseCode.SUCCESS) 
            {
                MMessageBox.Error("保存失败");
            }
        }

        /// <summary>
        /// 查看收藏夹列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemFaviritesList_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;

            FavoritesParagraphSource favoritesParagraphSource = new FavoritesParagraphSource(this.serviceAgent);
            favoritesParagraphSource.Session = this.Session;

            FavoritesVM favoritesVM = new FavoritesVM(favoritesParagraphSource, this.videoTerminal);
            favoritesVM.SendToAllTerminalDlg = mainWindow.SendToAllTerminal;

            ParagraphsWindow paragraphsWindow = new ParagraphsWindow(favoritesVM);
            paragraphsWindow.Title = "收藏夹列表";
            paragraphsWindow.Owner = mainWindow;
            paragraphsWindow.Show();
        }

        #endregion

        #region IDrawingWindow

        public IDrawingCanvas CreateCanvas()
        {
            DrawingCanvas document = new DrawingCanvas();
            return document;
        }

        public void InsertCanvas(int index, IDrawingCanvas document)
        {
            this.Dispatcher.Invoke(() =>
            {
                GridCanvasList.Children.Insert(0, document as UIElement);
            });
        }

        public void VisibleCanvas(IDrawingCanvas document, bool visible)
        {
            base.Dispatcher.Invoke(() =>
            {
                (document as FrameworkElement).Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        public void GetScrollInfo(ref VTScrollInfo scrollInfo)
        {
            scrollInfo.ScrollMax = (int)SliderScrolbar.Maximum;
            scrollInfo.ScrollValue = (int)SliderScrolbar.Value;
        }

        /// <summary>
        /// 更新滚动信息
        /// </summary>
        /// <param name="maximum">滚动条的最大值</param>
        public void SetScrollInfo(VTScrollInfo scrollInfo)
        {
            SliderScrolbar.ValueChanged -= this.SliderScrolbar_ValueChanged;

            if (SliderScrolbar.Minimum != scrollInfo.ScrollMin)
            {
                SliderScrolbar.Minimum = scrollInfo.ScrollMin;
            }

            if (SliderScrolbar.Maximum != scrollInfo.ScrollMax)
            {
                SliderScrolbar.Maximum = scrollInfo.ScrollMax;
            }

            if (SliderScrolbar.Value != scrollInfo.ScrollValue)
            {
                SliderScrolbar.Value = scrollInfo.ScrollValue;
            }

            SliderScrolbar.ValueChanged += this.SliderScrolbar_ValueChanged;
        }

        public void SetScrollVisible(bool visible)
        {
            base.Dispatcher.Invoke(() =>
            {
                if (visible)
                {
                    SliderScrolbar.Visibility = Visibility.Visible;
                }
                else
                {
                    SliderScrolbar.Visibility = Visibility.Collapsed;
                }
            });
        }

        public VTypeface GetTypeface(VTextStyle textStyle)
        {
            Typeface typeface = DrawingUtils.GetTypeface(textStyle);
            FormattedText formattedText = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                textStyle.FontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);

            return new VTypeface() 
            {
                Height = formattedText.Height,
                Width = formattedText.WidthIncludingTrailingWhitespace
            };
        }

        public VTRect GetDisplayRect()
        {
            Point leftTop = GridCanvasList.PointToScreen(new Point(0, 0));
            return new VTRect(leftTop.X, leftTop.Y, GridCanvasList.ActualWidth, GridCanvasList.ActualHeight);
        }

        #endregion

        #region SessionContent

        protected override int OnOpen(OpenedSessionVM viewModel)
        {
            this.clipboard = new VTClipboard()
            {
                MaximumHistory = this.Session.GetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY)
            };

            this.videoTerminal = viewModel as VideoTerminal;
            this.videoTerminal.Open();

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            // 停止对终端的日志记录
            MTermApp.Context.LoggerManager.Stop(this.videoTerminal);

            // 释放剪贴板
            this.clipboard.Release();

            this.videoTerminal.Close();
        }

        #endregion
    }
}
