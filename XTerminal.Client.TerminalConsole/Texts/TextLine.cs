using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    public class TextLine : FrameworkElement
    {
        #region 事件

        public event Action<object, TextItem> TextSelected;

        #endregion

        #region 实例变量

        private DrawingVisual textVisual;
        private List<TextItem> textList;

        #endregion

        #region 属性

        public List<TextItem> TextList
        {
            get
            {
                return this.textList;
            }
        }

        #endregion

        #region 构造方法

        public TextLine()
        {
            base.HorizontalAlignment = HorizontalAlignment.Left;
            base.VerticalAlignment = VerticalAlignment.Top;
            base.Height = DefaultValues.FontSize;
            base.Margin = DefaultValues.LineMargin;

            this.textVisual = new DrawingVisual();
            this.textList = new List<TextItem>();
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

        internal void Draw()
        {
            DrawingContext context = this.textVisual.RenderOpen();
            foreach (TextItem text in this.textList)
            {
                context.DrawText(text, new Point(0, 0));
                Console.WriteLine("draw");
            }
            context.Close();

            base.Height = this.textList.Max(t => t.Height);
            base.Width = this.textList.Sum(t => t.WidthIncludingTrailingWhitespace);
        }

        #endregion
    }
}