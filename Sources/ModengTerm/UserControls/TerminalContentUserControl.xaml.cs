using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
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
using System.Windows.Controls.Primitives;
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

        private ShellSessionVM shellSession;
        private IVideoTerminal videoTerminal;

        /// <summary>
        /// 提供剪贴板功能
        /// </summary>
        private VTClipboard clipboard;

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        private ServiceAgent serviceAgent;

        /// <summary>
        /// 记录当鼠标按下的时候的文本行
        /// </summary>
        private VTextLine mouseDownLine;

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

        private void SaveToFile(ParagraphTypeEnum paragraphType)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            saveFileDialog.FileName = string.Format("{0}_{1}", this.Session.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if ((bool)saveFileDialog.ShowDialog())
            {
                LogFileTypeEnum fileType = this.FilterIndex2FileType(saveFileDialog.FilterIndex);

                try
                {
                    VTParagraph paragraph = this.videoTerminal.CreateParagraph(paragraphType, fileType);
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

            // 记录鼠标所在行
            this.mouseDownLine = this.videoTerminal.ActiveDocument.GetCursorLine();
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
            this.shellSession.SendInput(text);
        }

        private void MenuItemSelectAll_Click(object sender, RoutedEventArgs e)
        {
            this.videoTerminal.SelectAll();
        }

        #region 保存日志

        private void MenuItemSaveDocument_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ParagraphTypeEnum.Viewport);
        }

        private void MenuItemSaveSelected_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ParagraphTypeEnum.Selected);
        }

        private void MenuItemSaveAll_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ParagraphTypeEnum.AllDocument);
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

            ClipboardVM clipboardVM = new ClipboardVM(clipboardParagraphSource, this.shellSession);
            clipboardVM.SendToAllTerminalDlg = mainWindow.SendToAllTerminal;

            ParagraphsWindow paragraphsWindow = new ParagraphsWindow(clipboardVM);
            paragraphsWindow.Title = "剪贴板历史";
            paragraphsWindow.Owner = mainWindow;
            paragraphsWindow.Show();
        }

        #region 收藏夹

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
                Typeface = this.videoTerminal.ActiveDocument.Typeface,
                SessionID = this.Session.ID,
                StartCharacterIndex = paragraph.StartCharacterIndex,
                EndCharacterIndex = paragraph.EndCharacterIndex,
                CharacterList = paragraph.CharacterList,
                CreationTime = paragraph.CreationTime,
            };

            int code = this.serviceAgent.AddFavorites(favorites);
            if (code != ResponseCode.SUCCESS)
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

            FavoritesVM favoritesVM = new FavoritesVM(favoritesParagraphSource, this.shellSession);
            favoritesVM.SendToAllTerminalDlg = mainWindow.SendToAllTerminal;

            ParagraphsWindow paragraphsWindow = new ParagraphsWindow(favoritesVM);
            paragraphsWindow.Title = "收藏夹列表";
            paragraphsWindow.Owner = mainWindow;
            paragraphsWindow.Show();
        }

        #endregion

        #region 书签

        private void MeuItemAddBookmark_Click(object sender, RoutedEventArgs e)
        {
            if (this.mouseDownLine == null)
            {
                return;
            }

            this.shellSession.SetBookmarkState(this.mouseDownLine, VTBookmarkStates.Enabled);
        }

        private void MeuItemRemoveBookmark_Click(object sender, RoutedEventArgs e)
        {
            if (this.mouseDownLine == null)
            {
                return;
            }

            this.shellSession.SetBookmarkState(this.mouseDownLine, VTBookmarkStates.None);
        }

        private void MenuItemBookmarkList_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;

            BookmarkParagraphSource bookmarkParagraphSource = new BookmarkParagraphSource(this.shellSession.BookmarkMgr);

            BookmarksVM bookmarksVM = new BookmarksVM(bookmarkParagraphSource, this.shellSession);
            bookmarksVM.SendToAllTerminalDlg = mainWindow.SendToAllTerminal;

            ParagraphsWindow paragraphsWindow = new ParagraphsWindow(bookmarksVM);
            paragraphsWindow.Title = "书签列表";
            paragraphsWindow.Owner = mainWindow;
            paragraphsWindow.Show();
        }

        private void MenuItemDisplayBookmark_Click(object sender, RoutedEventArgs e)
        {
            this.videoTerminal.SetBookmarkVisible(true);
        }

        private void MenuItemHidenBookmark_Click(object sender, RoutedEventArgs e)
        {
            this.videoTerminal.SetBookmarkVisible(false);
        }

        #endregion

        #endregion

        #region IDrawingWindow

        public IDrawingDocument CreateCanvas()
        {
            DrawingDocument drawingDocument = new DrawingDocument();
            return drawingDocument;
        }

        public void InsertCanvas(int index, IDrawingDocument document)
        {
            this.Dispatcher.Invoke(() =>
            {
                FrameworkElement element = document as FrameworkElement;
                GridDocumentList.Children.Insert(0, element);

                // 添加到子节点里之后马上加载模板，不然后面新建DrawingObject的时候模板还没加载，找不到drawArea
                element.ApplyTemplate();
            });
        }

        public void RemoveDocument(IDrawingDocument document)
        {
            DrawingDocument drawingDocument = document as DrawingDocument;
            GridDocumentList.Children.Remove(drawingDocument);
        }

        public void VisibleCanvas(IDrawingDocument document, bool visible)
        {
            base.Dispatcher.Invoke(() =>
            {
                (document as FrameworkElement).Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        public VTypeface GetTypeface(double fontSize, string fontFamily)
        {
            Typeface typeface = DrawingUtils.GetTypeface(fontFamily);
            FormattedText formattedText = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                fontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);

            return new VTypeface()
            {
                FontSize = fontSize,
                FontFamily = fontFamily,
                Height = formattedText.Height,
                SpaceWidth = formattedText.WidthIncludingTrailingWhitespace
            };
        }

        public VTSize GetSize()
        {
            return new VTSize(GridDocumentList.ActualWidth, GridDocumentList.ActualHeight);
        }

        #endregion

        #region SessionContent

        protected override int OnOpen(OpenedSessionVM viewModel)
        {
            this.clipboard = new VTClipboard()
            {
                MaximumHistory = this.Session.GetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY)
            };

            this.shellSession = viewModel as ShellSessionVM;
            this.shellSession.Open();

            this.videoTerminal = this.shellSession.VideoTerminal;

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            // 停止对终端的日志记录
            MTermApp.Context.LoggerManager.Stop(this.videoTerminal);

            // 释放剪贴板
            this.clipboard.Release();

            this.shellSession.Close();
            this.videoTerminal = null;
        }

        #endregion
    }
}
