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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoTerminal.Base;
using VideoTerminal.Parser;
using VTInterface;

namespace VideoTerminal
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class VTConsole : UserControl, IVideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTConsole");

        #endregion

        #region 实例变量

        private Paragraph paragraph;
        private int cursorRow = 0;
        private int cursorColumn = 0;

        private Run lastInline;

        private Brush defaultForeground;
        private Brush defaultBackground;

        #endregion

        #region 属性

        public TextPointer CurrentCaretPosition
        {
            get
            {
                return RichTextBox.CaretPosition;
            }
        }

        public int CursorRow { get { return this.cursorRow; } set { this.cursorRow = value; } }

        public int CursorColumn { get { return this.cursorColumn; } set { this.cursorColumn = value; } }

        #endregion

        #region 构造方法

        public VTConsole()
        {
            InitializeComponent();

            this.InitializeConsole();
        }

        #endregion

        #region 实例方法

        private void InitializeConsole()
        {
            this.defaultForeground = RichTextBox.Foreground;
            this.defaultBackground = Brushes.Transparent;

            this.paragraph = new Paragraph();
            RichTextBox.IsUndoEnabled = false;
            RichTextBox.IsReadOnlyCaretVisible = false;
            RichTextBox.IsReadOnly = true;
            RichTextBox.Document.Blocks.Clear();
            RichTextBox.Document.Blocks.Add(this.paragraph);
            RichTextBox.Document.IsEnabled = true;
            RichTextBox.PreviewTextInput += RichTextBox_PreviewTextInput;
        }

        ///// <summary>
        ///// TextPointer
        ///// </summary>
        ///// <param name="column"></param>
        ///// <returns></returns>
        //private TextPointer GetTextPointer(int row, int column)
        //{
        //    RichTextBox.CaretPosition.GetLineStartPosition
        //    RichTextBox.CaretPosition.GetPositionAtOffset(column);
        //}

        private void InsertTextAtPosition(string text, int column, int row)
        {
            //this._paragraph.InsertTextAtPosition(text, column, row);
            //TextPointer textPosition = this._paragraph.ContentStart;
            //textPosition = textPosition.GetLineStartPosition(row);
            //textPosition = textPosition.GetPositionAtOffset(column, LogicalDirection.Forward);
            //textPosition.InsertTextInRun(text);
        }

        private void EraseText(int row, int startCol, int endCol)
        {

        }

        private Run CreateRun()
        {
            Run result = new Run();
            TextOptions.SetTextFormattingMode(result, TextFormattingMode.Display);
            this.paragraph.Inlines.Add(result);
            return result;
        }

        private Brush TextColor2Brush(TextColor textColor)
        {
            switch (textColor)
            {
                case TextColor.DARK_BLACK: return Brushes.Black;
                case TextColor.DARK_BLUE: return Brushes.DarkBlue;
                case TextColor.DARK_CYAN: return Brushes.DarkCyan;
                case TextColor.DARK_GREEN: return Brushes.DarkGreen;
                case TextColor.DARK_MAGENTA: return Brushes.DarkMagenta;
                case TextColor.DARK_RED: return Brushes.DarkRed;
                case TextColor.DARK_WHITE: return Brushes.White;
                case TextColor.DARK_YELLOW: return Brushes.Orange;

                case TextColor.BRIGHT_BLACK: return Brushes.Silver;
                case TextColor.BRIGHT_BLUE: return Brushes.LightBlue;
                case TextColor.BRIGHT_CYAN: return Brushes.LightCyan;
                case TextColor.BRIGHT_GREEN: return Brushes.LightGreen;
                case TextColor.BRIGHT_MAGENTA: return Brushes.Magenta;
                case TextColor.BRIGHT_RED: return Brushes.Red;
                case TextColor.BRIGHT_WHITE: return Brushes.White;
                case TextColor.BRIGHT_YELLOW: return Brushes.LightYellow;

                default:
                    throw new NotImplementedException();
            }
        }

        private Brush CreateBrush(byte r, byte g, byte b)
        {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        #endregion

        #region 事件处理器

        private void Test(object sender, TextCompositionEventArgs e)
        {
            Console.WriteLine(e.Text);
        }

        private void RichTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //byte[] data;
            //if (this.VT.HandleInputWideChar(e.Text, out data))
            //{
            //    if (!this.vt.Socket.Write(data))
            //    {
            //        logger.ErrorFormat("向终端发送数据失败");
            //    }
            //}
        }

        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.ImeProcessed)
            //{
            //    Console.WriteLine(e.ImeProcessedKey);
            //}
            //else
            //{
            //    Console.WriteLine(e.Key);
            //}

            //if (e.Key == Key.ImeProcessed)
            //{
            //}
            //else
            //{
            //    byte[] data;
            //    if (this.VT.HandleInputChar(e, out data))
            //    {
            //        if (!this.vt.Socket.Write(data))
            //        {
            //            logger.ErrorFormat("向终端发送数据失败");
            //        }
            //    }

            //    e.Handled = true;
            //}
        }

        #endregion

        #region IVideoTerminal

        public void PerformAction(List<VTAction> vtActions)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.lastInline = this.CreateRun();

                foreach (VTAction vtAction in vtActions)
                {
                    switch (vtAction.Type)
                    {
                        case VTActions.Foreground:
                            {
                                this.lastInline.Foreground = this.TextColor2Brush((TextColor)vtAction.Data);
                                break;
                            }

                        case VTActions.Background:
                            {
                                this.lastInline.Background = this.TextColor2Brush((TextColor)vtAction.Data);
                                break;
                            }

                        case VTActions.ForegroundRGB:
                            {
                                this.lastInline.Foreground = this.CreateBrush(vtAction.R, vtAction.G, vtAction.B);
                                break;
                            }

                        case VTActions.BackgroundRGB:
                            {
                                this.lastInline.Background = this.CreateBrush(vtAction.R, vtAction.G, vtAction.B);
                                break;
                            }

                        case VTActions.Blink:
                            {
                                break;
                            }

                        case VTActions.BlinkUnset:
                            {
                                break;
                            }

                        case VTActions.Bold:
                            {
                                this.lastInline.FontWeight = FontWeights.Bold;
                                break;
                            }

                        case VTActions.BoldUnset:
                            {
                                this.lastInline.FontWeight = FontWeights.Normal;
                                break;
                            }

                        case VTActions.CrossedOut:
                            {
                                break;
                            }

                        case VTActions.CrossedOutUnset:
                            {
                                break;
                            }

                        case VTActions.DefaultAttributes:
                            {
                                break;
                            }

                        case VTActions.DefaultBackground:
                            {
                                break;
                            }

                        case VTActions.DefaultForeground:
                            {
                                break;
                            }

                        case VTActions.DoublyUnderlined:
                            {
                                break;
                            }

                        case VTActions.DoublyUnderlinedUnset:
                            {
                                break;
                            }

                        case VTActions.Faint:
                            {
                                break;
                            }

                        case VTActions.FaintUnset:
                            {
                                break;
                            }

                        case VTActions.Invisible:
                            {
                                break;
                            }

                        case VTActions.InvisibleUnset:
                            {
                                break;
                            }

                        case VTActions.Italics:
                            {
                                this.lastInline.FontStyle = FontStyles.Italic;
                                break;
                            }

                        case VTActions.ItalicsUnset:
                            {
                                this.lastInline.FontStyle = FontStyles.Normal;
                                break;
                            }

                        case VTActions.Overlined:
                            {
                                this.lastInline.TextDecorations = TextDecorations.OverLine;
                                break;
                            }

                        case VTActions.OverlinedUnset:
                            {
                                this.lastInline.TextDecorations = null;
                                break;
                            }

                        case VTActions.ReverseVideo:
                            {
                                break;
                            }

                        case VTActions.ReverseVideoUnset:
                            {
                                break;
                            }

                        case VTActions.Underline:
                            {
                                this.lastInline.TextDecorations = TextDecorations.Underline;
                                break;
                            }

                        case VTActions.UnderlineUnset:
                            {
                                this.lastInline.TextDecorations = null;
                                break;
                            }

                        default:
                            logger.WarnFormat("未实现VTAction, {0}", vtAction.Type);
                            break;
                    }
                }
            }));
        }

        public void PerformAction(VTAction vtAction)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                switch (vtAction.Type)
                {
                    case VTActions.PlayBell:
                        {
                            // 播放响铃
                            break;
                        }

                    case VTActions.Print:
                        {
                            if (this.lastInline == null || !(this.paragraph.Inlines.LastInline is Run))
                            {
                                this.lastInline = this.CreateRun();
                            }

                            string text = vtAction.Data.ToString();
                            this.lastInline.Text += text;
                            break;
                        }

                    default:
                        logger.WarnFormat("未实现VTAction, {0}", vtAction.Type);
                        break;
                }
            }));
        }

        public void CursorBackward(int distance)
        {
            logger.WarnFormat("CursorBackward");
        }

        public void CursorForward(int distance)
        {
            logger.WarnFormat("CursorForward");
        }

        public void ForwardTab()
        {
            logger.WarnFormat("ForwardTab");
        }

        public void CarriageReturn()
        {
        }

        public void LineFeed()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.paragraph.Inlines.Add(new LineBreak());
            }));
        }

        public ICursorState CursorSaveState()
        {
            logger.WarnFormat("CursorSaveState");
            return null;
        }

        public void CursorRestoreState(ICursorState state)
        {
            logger.WarnFormat("CursorRestoreState");
        }

        public bool CursorVisibility(bool visible)
        {
            RichTextBox.IsReadOnlyCaretVisible = visible;
            return true;
        }

        public void CursorPosition(int row, int column)
        {
            logger.WarnFormat("CursorPosition");
        }

        public IPresentationDevice CreatePresentationDevice()
        {
            logger.WarnFormat("CreatePresentationDevice");
            return null;
        }

        public void DeletePresentationDevice(IPresentationDevice device)
        {
            logger.WarnFormat("DeletePresentationDevice");
        }

        public bool SwitchPresentationDevice(IPresentationDevice activeDevice)
        {
            logger.WarnFormat("SwitchPresentationDevice");
            return true;
        }

        public IPresentationDevice GetActivePresentationDevice()
        {
            logger.WarnFormat("GetActivePresentationDevice");
            return null;
        }

        #endregion
    }
}