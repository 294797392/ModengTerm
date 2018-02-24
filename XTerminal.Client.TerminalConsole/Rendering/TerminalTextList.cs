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
        private int previewMeasuredTotalLines; // 上次测量控件应该占据的空间大小的时候的行数
        private ScrollViewer scrollViewer;

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
        }

        internal void HandleCommandInput(IEnumerable<IEscapeSequencesCommand> cmds)
        {
        }

        internal void HandleTextInput(TextCompositionEventArgs e)
        {
            if (e.Text == "\n" || e.Text == "\r" || e.Text == "\r\n" || this.currentLine == null)
            {
                // 处理换行符
            }
            else
            {
                // 处理普通字符
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