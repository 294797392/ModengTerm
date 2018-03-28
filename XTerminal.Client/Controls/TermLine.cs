using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using XTerminal.Client.TerminalConsole;

namespace XTerminal.Controls
{
    public class TermLine : FrameworkElement
    {
        #region 事件

        public event Action<object, TermText> TextSelected;

        #endregion

        #region 实例变量

        private DrawingVisual textVisual;
        private List<TermText> textList;

        #endregion

        #region 构造方法

        public TermLine()
        {
            base.HorizontalAlignment = HorizontalAlignment.Left;
            base.VerticalAlignment = VerticalAlignment.Top;

            this.textVisual = new DrawingVisual();
            this.textList = new List<TermText>();
            base.AddVisualChild(this.textVisual);
        }

        #endregion

        #region 重写方法

        protected override Visual GetVisualChild(int index)
        {
            return this.textVisual;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            //HitTestResult result = VisualTreeHelper.HitTest(this.textVisual, e.GetPosition(this));
            //if (result != null)
            //{

            //}

            if (e.ClickCount == 2)
            {

            }
            else
            {

            }
        }

        #endregion

        #region 实例方法

        public void Draw()
        {
            DrawingContext context = this.textVisual.RenderOpen();
            Typeface face = new Typeface("宋体");
            context.DrawText(new FormattedText("asdasdasd", System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, face, 14, Brushes.Red), new Point(0, 0));
            foreach (TermText text in this.textList)
            {
                context.DrawText(text, new Point(0, 0));
                Console.WriteLine("draw");
            }
            context.Close();

            Console.WriteLine(this.textVisual.ContentBounds.Height);
            Console.WriteLine(this.textVisual.ContentBounds.Width);

            //base.Height = this.textList.Max(t => t.Height);
            //base.Width = this.textList.Sum(t => t.WidthIncludingTrailingWhitespace);
        }

        #endregion
    }
}