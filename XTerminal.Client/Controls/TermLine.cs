using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XTerminal.Client.TerminalConsole;

namespace XTerminal.Controls
{
    internal class TermLine : Control
    {
        #region 事件

        public event Action<object, TermText> TextSelected;

        #endregion

        #region 实例变量

        private List<TermText> textList;

        #endregion

        #region 构造方法

        public TermLine()
        {
            this.textList = new List<TermText>();
        }

        #endregion

        #region 重写方法

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

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            //Console.WriteLine("OnRender");

            //Typeface face = new Typeface("宋体");
            //drawingContext.DrawText(new FormattedText("asdasdasd", System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, face, 50, Brushes.Red), new Point(0, 0));
            foreach (TermText text in this.textList)
            {
                drawingContext.DrawText(text, new Point(0, 0));
                Console.WriteLine("draw");
            }
        }

        #endregion

        #region 实例方法

        public void Draw()
        {
            //base.Height = this.textList.Max(t => t.Height);
            //base.Width = this.textList.Sum(t => t.WidthIncludingTrailingWhitespace);
        }

        public void SetTermTextList(IEnumerable<TermText> textList)
        {
            this.textList.Clear();
            this.textList.AddRange(textList);
        }

        #endregion
    }
}