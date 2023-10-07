using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Rendering;
using ModengTerm.Terminal;
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
    public partial class TerminalContentUserControl : SessionContent, IDrawingVideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #endregion

        #region 实例变量

        private UserInput userInput;

        private VideoTerminal videoTerminal;
        private FindVM vtFind;

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

            this.userInput = new UserInput();
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
        
        private void SaveToFile(ContentScopeEnum saveMode)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            saveFileDialog.FileName = string.Format("{0}_{1}", this.videoTerminal.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if ((bool)saveFileDialog.ShowDialog())
            {
                LogFileTypeEnum fileType = this.FilterIndex2FileType(saveFileDialog.FilterIndex);

                try
                {
                    string text = this.videoTerminal.CreateContent(saveMode, fileType);
                    File.WriteAllText(saveFileDialog.FileName, text);
                }
                catch (Exception ex)
                {
                    logger.Error("保存日志异常", ex);
                    MessageBoxUtils.Error("保存失败");
                }
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 向SSH服务器发送输入
        /// </summary>
        /// <param name="userInput"></param>
        private void SendUserInput(UserInput userInput)
        {
            if (MenuItemSendAll.IsChecked)
            {
                foreach (VideoTerminal vt in MTermApp.Context.OpenedTerminals)
                {
                    if (vt == this.videoTerminal)
                    {
                        continue;
                    }

                    vt.SendInput(userInput);
                }
            }
            else
            {
                this.videoTerminal.SendInput(userInput);
            }
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 输入中文的时候会触发该事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            this.userInput.CapsLock = Console.CapsLock;
            this.userInput.Key = VTKeys.GenericText;
            this.userInput.Text = e.Text;
            this.userInput.Modifiers = VTModifierKeys.None;

            this.SendUserInput(this.userInput);

            e.Handled = true;

            //Console.WriteLine(e.Text);
        }

        /// <summary>
        /// 从键盘上按下按键的时候会触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            //Console.WriteLine(e.Key);

            if (e.Key == Key.ImeProcessed)
            {
                // 这些字符交给输入法处理了
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Tab:
                    case Key.Up:
                    case Key.Down:
                    case Key.Left:
                    case Key.Right:
                    case Key.Space:
                        {
                            // 防止焦点移动到其他控件上了
                            e.Handled = true;
                            break;
                        }
                }

                if (e.Key != Key.ImeProcessed)
                {
                    e.Handled = true;
                }

                VTKeys vtKey = DrawingUtils.ConvertToVTKey(e.Key);
                this.userInput.CapsLock = Console.CapsLock;
                this.userInput.Key = vtKey;
                this.userInput.Text = null;
                this.userInput.Modifiers = (VTModifierKeys)e.KeyboardDevice.Modifiers;
                this.SendUserInput(this.userInput);
            }
        }

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
            this.videoTerminal.CopySelection();
        }

        private void MenuItemPaste_Click(object sender, RoutedEventArgs e)
        {
            this.videoTerminal.Paste();
        }

        private void MenuItemSelectAll_Click(object sender, RoutedEventArgs e)
        {
            this.videoTerminal.SelectAll();
        }

        #region 保存日志

        private void MenuItemSaveDocument_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ContentScopeEnum.SaveDocument);
        }

        private void MenuItemSaveSelected_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ContentScopeEnum.SaveSelected);
        }

        private void MenuItemSaveAll_Click(object sender, RoutedEventArgs e)
        {
            this.SaveToFile(ContentScopeEnum.SaveAll);
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

        private void MenuItemDrawingMode_CheckedChanged(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemFind_Click(object sender, RoutedEventArgs e)
        {
            FindWindow findWindow = new FindWindow(this.vtFind);
            findWindow.Owner = Window.GetWindow(this);
            findWindow.Show();
        }

        #endregion

        #region IVideoTerminalRenderer

        public IDrawingDocument CreateDocument()
        {
            DrawingDocument document = new DrawingDocument();
            return document;
        }

        public void InsertDocument(int index, IDrawingDocument document)
        {
            this.Dispatcher.Invoke(() =>
            {
                GridCanvasList.Children.Insert(0, document as UIElement);
            });
        }

        public void RemoveDocument(IDrawingDocument canvas)
        {
            base.Dispatcher.Invoke(() =>
            {
                GridCanvasList.Children.Remove(canvas as UIElement);
            });
        }

        public void VisibleDocument(IDrawingDocument document, bool visible)
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

        public VTextMetrics MeasureText(string text, double fontSize, string fontFamily)
        {
            Typeface typeface = new Typeface(new FontFamily(fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                fontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);

            VTextMetrics metrics = new VTextMetrics()
            {
                Height = formattedText.Height,
                Width = formattedText.WidthIncludingTrailingWhitespace
            };

            return metrics;
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
            this.videoTerminal = viewModel as VideoTerminal;
            this.videoTerminal.Open();

            this.vtFind = new FindVM(this.videoTerminal);

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            this.videoTerminal.Close();
        }

        #endregion

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (!element.IsMouseCaptured)
            {
                Mouse.Capture(element, CaptureMode.SubTree);
            }
            else
            {
                if (e.OriginalSource != element)
                {
                    Mouse.Capture(null);
                    ToggleButtonLoggerSetting.IsChecked = false;
                }
            }
        }
    }
}
