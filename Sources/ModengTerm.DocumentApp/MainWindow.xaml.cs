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

namespace ModengTerm.DocumentApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VTDocument document;
        private VTInput input = new VTInput();

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
                SpaceWidth = formattedText.WidthIncludingTrailingWhitespace,
                BackgroundColor = String.Empty,
                ForegroundColor = "0,0,0"
            };

            VTDocumentOptions options = new VTDocumentOptions()
            {
                DrawingObject = Document,
                ViewportRow = 10,
                ViewportColumn = 100,
                CursorColor = "0,0,0",
                CursorStyle = VTCursorStyles.Line,
                ScrollbackMax = 100,
                Typeface = typeface1,
                SelectionColor = "114,213,252"
            };
            this.document = new VTDocument(options);
            this.document.Initialize();
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            this.input.Text = e.Text;
            this.input.Type = VTInputTypes.TextInput;

            this.document.EventInput.OnInput(this.input);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            this.input.Type = VTInputTypes.KeyInput;
            this.input.Key = (VTKeys)e.Key;
            this.document.EventInput.OnInput(this.input);

            e.Handled = true;

            //if (e.Key == Key.ImeProcessed)
            //{
            //    // 这些字符交给输入法处理了
            //}
            //else
            //{
            //    switch (e.Key)
            //    {
            //        case Key.Tab:
            //        case Key.Up:
            //        case Key.Down:
            //        case Key.Left:
            //        case Key.Right:
            //        case Key.Space:
            //            {
            //                // 防止焦点移动到其他控件上了
            //                e.Handled = true;
            //                break;
            //            }
            //    }

            //    this.document.EventInput.OnTextInput(e.Key.ToString());
            //}
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Point position = e.GetPosition(Document);

            this.document.EventInput.OnMouseDown(new VTPoint(position.X, position.Y), e.ClickCount);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            Point position = e.GetPosition(Document);

            this.document.EventInput.OnMouseUp(new VTPoint(position.X, position.Y));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point position = e.GetPosition(Document);

            this.document.EventInput.OnMouseMove(new VTPoint(position.X, position.Y));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            this.document.EventInput.OnMouseWheel(e.Delta > 0);
        }
    }
}
