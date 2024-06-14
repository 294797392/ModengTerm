using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ModengTerm.Document.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VTDocument document;
        private VTInputData input = new VTInputData();
        private InputEventImpl inputEventImpl;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Typeface typeface = DrawingUtils.GetTypeface("宋体");
            FormattedText formattedText = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                12, Brushes.Black, null, TextFormattingMode.Display, DrawingUtils.PixelPerDpi);

            VTypeface typeface1 = new VTypeface()
            {
                FontSize = 12,
                FontFamily = "宋体",
                Height = formattedText.Height,
                Width = formattedText.WidthIncludingTrailingWhitespace,
                BackgroundColor = String.Empty,
                ForegroundColor = "0,0,0,255"
            };

            VTDocumentOptions options = new VTDocumentOptions()
            {
                Controller = WPFDocument,
                ViewportRow = 10,
                ViewportColumn = 100,
                CursorColor = "0,0,0,255",
                CursorStyle = VTCursorStyles.Line,
                ScrollbackMax = 100,
                Typeface = typeface1,
                SelectionColor = "114,213,252,180"
            };
            this.document = new VTDocument(options);
            this.document.Initialize();

            this.inputEventImpl = new InputEventImpl(this.document);
        }

        private void HandleMouseCapture(MouseData mouseData)
        {
            if (mouseData.IsMouseCaptured)
            {
                if (this.IsMouseCaptured)
                {
                    return;
                }

                this.CaptureMouse();
            }
            else
            {
                if (!this.IsMouseCaptured)
                {
                    return;
                }

                this.ReleaseMouseCapture();
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            //foreach (char c in e.Text)
            //{
            //    VTCharacter character = VTCharacter.Create(c, 2);

            //    this.document.PrintCharacter(character);
            //}

            //this.document.RequestInvalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

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
                    case Key.LeftShift:
                        {
                            // 防止焦点移动到其他控件上了
                            e.Handled = true;
                            return;
                        }

                    case Key.Enter: 
                        {
                            this.document.SetCursor(this.document.Cursor.Row + 1, 0);
                            break;
                        }

                    default:
                        {
                            VTCharacter character = VTCharacter.Create(char.Parse(e.Key.ToString()), 1);
                            this.document.PrintCharacter(character);
                            this.document.SetCursor(this.document.Cursor.Row, this.document.Cursor.Column + 1);
                            break;
                        }
                }

                this.document.RequestInvalidate();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Point position = e.GetPosition(WPFDocument);

            MouseData mouseData = new MouseData(position.X, position.Y, e.ClickCount);

            this.document.EventInput.OnMouseDown(mouseData);

            this.HandleMouseCapture(mouseData);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            Point position = e.GetPosition(WPFDocument);

            MouseData mouseData = new MouseData(position.X, position.Y, e.ClickCount);

            this.document.EventInput.OnMouseUp(mouseData);

            this.HandleMouseCapture(mouseData);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point position = e.GetPosition(WPFDocument);

            MouseData mouseData = new MouseData(position.X, position.Y, 0);

            this.document.EventInput.OnMouseMove(mouseData);

            this.HandleMouseCapture(mouseData);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            this.document.EventInput.OnMouseWheel(e.Delta > 0);
        }
    }
}
