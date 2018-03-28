using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminal.Terminal;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    public class TextList : ItemsControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalTextPanel");

        #endregion

        #region 实例变量

        private ObservableCollection<object> lineObjects;
        private TextLine currentLine;

        #endregion

        #region 属性

        public static readonly DependencyProperty LineMarginProperty = DependencyProperty.Register("LineMargin", typeof(Thickness), typeof(TextList), new PropertyMetadata(DefaultValues.LineMargin));
        public Thickness LineMargin
        {
            get
            {
                return (Thickness)base.GetValue(LineMarginProperty);
            }
            set
            {
                base.SetValue(LineMarginProperty, value);
            }
        }

        #endregion

        #region 构造方法

        public TextList()
        {
            this.InitializeTextList();
        }

        #endregion

        #region 实例方法

        private void InitializeTextList()
        {
            //base.ItemsSource = this.lines;
            this.lineObjects = new ObservableCollection<object>();
            base.ItemsSource = this.lineObjects;

            this.lineObjects.Add(new object());
        }

        public void HandleTextInput(TextCompositionEventArgs e)
        {
            if (e.Text.Length == 1)
            {
                char c = e.Text[0];
                if (c == '\n' || c == '\r' || this.currentLine == null)
                {
                    this.currentLine = new TextLine();
                    base.Items.Add(this.currentLine);
                }
                else
                {
                    TextItem text = null;
                    int textCount = this.currentLine.TextList.Count;
                    if (textCount == 0)
                    {
                        Typeface face = new Typeface("宋体");
                        text = new TextItem(e.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, 12, Brushes.Black);
                        this.currentLine.TextList.Add(text);
                    }
                    else
                    {
                        Typeface face = new Typeface("宋体");
                        var exitText = this.currentLine.TextList[textCount - 1];
                        text = new TextItem(exitText.Text + e.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, 12, Brushes.Black);
                        this.currentLine.TextList[textCount - 1] = text;
                    }
                    this.currentLine.Draw();
                }
            }
            else
            {

            }
        }

        internal TextLine GetCurrentTextLine()
        {
            if (this.currentLine != null)
            {
                return this.currentLine;
            }

            this.currentLine = new TextLine();
            base.Items.Add(this.currentLine);
            return this.currentLine;
        }

        internal void CreateTextLine()
        {
            var line = new TextLine();
            base.Items.Add(line);
            this.currentLine = line;
        }

        internal void AppendPlainText(TextLine line, string plainText)
        {
            TextItem text = null;
            int textCount = line.TextList.Count;
            if (textCount == 0)
            {
                Typeface face = new Typeface("宋体");
                text = new TextItem(plainText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, 12, Brushes.Black);
                line.TextList.Add(text);
            }
            else
            {
                Typeface face = new Typeface("宋体");
                var exitText = this.currentLine.TextList[textCount - 1];
                text = new TextItem(exitText.Text + plainText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, 12, Brushes.Black);
                line.TextList[textCount - 1] = text;
            }
            line.Draw();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Console.WriteLine(e.NewItems[0]);
            }
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion

        #region 事件处理器

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TextLine;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TextLine();
        }

        #endregion
    }
}