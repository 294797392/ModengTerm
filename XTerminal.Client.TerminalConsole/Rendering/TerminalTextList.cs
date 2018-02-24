using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class TerminalTextList : ItemsControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalTextPanel");

        #endregion

        #region 实例变量

        private ObservableCollection<TerminalLineItem> lines;
        private TerminalLineItem currentLine;

        #endregion

        #region 属性

        public static readonly DependencyProperty LineMarginProperty = DependencyProperty.Register("LineMargin", typeof(Thickness), typeof(TerminalTextList), new PropertyMetadata(DefaultValues.LineMargin));
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

        public static readonly DependencyProperty TerminalLinesProperty = DependencyProperty.Register("TerminalLines", typeof(IEnumerable<TerminalLineItem>), typeof(TerminalTextList), new PropertyMetadata());
        public IEnumerable<TerminalLineItem> TerminalLines
        {
            get
            {
                return (IEnumerable<TerminalLineItem>)base.GetValue(TerminalLinesProperty);
            }
            set
            {
                base.SetValue(TerminalLinesProperty, value);
            }
        }

        #endregion

        #region 构造方法

        public TerminalTextList()
        {
            this.InitializePanel();
        }

        #endregion

        #region 实例方法

        private void InitializePanel()
        {
            base.HorizontalAlignment = HorizontalAlignment.Stretch;
            base.VerticalAlignment = VerticalAlignment.Stretch;
            base.Background = Brushes.Transparent;
            this.lines = new ObservableCollection<TerminalLineItem>();
            //base.ItemsSource = this.lines;
        }

        internal void HandleCommandInput(IEnumerable<IEscapeSequencesCommand> cmds)
        {
        }

        public void HandleTextInput(TextCompositionEventArgs e)
        {
            if (e.Text.Length == 1)
            {
                char c = e.Text[0];
                if (c == '\n' || c == '\r' || this.currentLine == null)
                {
                    this.currentLine = new TerminalLineItem();
                    this.lines.Add(this.currentLine);
                    base.Items.Add(this.currentLine);
                }
                else
                {
                    TerminalLineText text = null;
                    int textCount = this.currentLine.TextList.Count;
                    if (textCount == 0)
                    {
                        Typeface face = new Typeface("宋体");
                        text = new TerminalLineText(e.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, 12, Brushes.Black);
                        this.currentLine.TextList.Add(text);
                    }
                    else
                    {
                        Typeface face = new Typeface("宋体");
                        var exitText = this.currentLine.TextList[textCount - 1];
                        text = new TerminalLineText(exitText.Text + e.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, face, 12, Brushes.Black);
                        this.currentLine.TextList[textCount - 1] = text;
                    }
                    this.currentLine.Draw();
                }
            }
            else
            {

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
            return item is TerminalLineItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TerminalLineItem();
        }

        #endregion
    }
}