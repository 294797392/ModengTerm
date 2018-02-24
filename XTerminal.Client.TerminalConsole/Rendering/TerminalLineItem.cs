using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    public class TerminalLineItem : FrameworkElement
    {
        private DrawingVisual textVisual;
        private List<TerminalLineText> textList;

        public List<TerminalLineText> TextList
        {
            get
            {
                return this.textList;
            }
        }

        public TerminalLineItem()
        {
            base.HorizontalAlignment = HorizontalAlignment.Left;
            base.VerticalAlignment = VerticalAlignment.Top;

            this.textVisual = new DrawingVisual();
            this.textList = new List<TerminalLineText>();
            base.AddVisualChild(this.textVisual);
        }

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

            HitTestResult result = VisualTreeHelper.HitTest(this.textVisual, e.GetPosition(this));
            if (result != null)
            {

            }
        }

        internal void Draw()
        {
            DrawingContext context = this.textVisual.RenderOpen();
            foreach (TerminalLineText text in this.textList)
            {
                context.DrawText(text, new Point(0, 0));
                Console.WriteLine("draw");
            }
            context.Close();

            base.Height = this.textList.Max(t => t.Height);
            base.Width = this.textList.Sum(t => t.WidthIncludingTrailingWhitespace);
        }
    }
}