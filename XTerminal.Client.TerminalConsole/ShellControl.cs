using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XTerminal.Client.TerminalConsole.Carets;
using XTerminal.Client.TerminalConsole.Rendering;
using XTerminal.Terminal;
using XTerminal.Terminal.EscapeSequences;

namespace XTerminal.Client.TerminalConsole
{
    public class ShellControl : Control
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ShellControl");

        #region 实例变量

        private Grid grid;
        private TextList textList;
        private CaretPanel caretPanel;
        private ITerminal currentTerminal;

        #endregion

        #region 属性

        public static readonly DependencyProperty TerminalProperty = DependencyProperty.Register("Terminal", typeof(ITerminal), typeof(ShellControl), new PropertyMetadata(null, new PropertyChangedCallback(TerminalPropertyChangedCallback)));
        public ITerminal Terminal
        {
            get
            {
                return base.GetValue(TerminalProperty) as ITerminal;
            }
            set
            {
                base.SetValue(TerminalProperty, value);
            }
        }

        #endregion

        #region 构造方法

        public ShellControl()
        {
            this.InitializeConsole();
        }

        #endregion

        #region 依赖属性回调

        private static void TerminalPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ShellControl).OnTerminalPropertyChanged(e.NewValue, e.OldValue);
        }

        protected void OnTerminalPropertyChanged(object newTerminal, object oldTerminal)
        {
            if (oldTerminal != null)
            {
                this.ReleaseTerminal(oldTerminal as ITerminal);
            }

            if (newTerminal != null)
            {
                this.InitializeTerminal(newTerminal as ITerminal);
                this.currentTerminal = newTerminal as ITerminal;
            }
        }

        #endregion

        #region 实例方法

        private void InitializeConsole()
        {
        }

        private void InitializeTerminal(ITerminal terminal)
        {
            terminal.CommandReceived += Terminal_CommandReceived;
            terminal.Initialize();
            terminal.Connect();
        }

        private void ReleaseTerminal(ITerminal terminal)
        {
            terminal.Disconnect();
            terminal.Release();
            terminal.CommandReceived -= Terminal_CommandReceived;
        }

        private void HandleCommand(IEnumerable<IEscapeSequencesCommand> commands)
        {
            foreach (var command in commands)
            {
                if (command == PredefineCommands.Enter)
                {

                }
                else if (command == PredefineCommands.NewLine)
                {
                    this.textList.CreateTextLine();
                }
                else if (command is NormalTextCommand)
                {
                    var text = (command as NormalTextCommand).Text;
                    var textLine = this.textList.GetCurrentTextLine();
                    this.textList.AppendPlainText(textLine, text);
                }
            }
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.textList = base.Template.FindName("PART_TextList", this) as TextList;
            this.caretPanel = base.Template.FindName("PART_CaretPanel", this) as CaretPanel;
        }

        #endregion

        #region 事件处理器

        private void Terminal_CommandReceived(object sender, IEnumerable<IEscapeSequencesCommand> commands)
        {
            base.Dispatcher.Invoke(new Action(() =>
            {
                this.HandleCommand(commands);
            }));
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            logger.DebugFormat("Text={0}, ControlText={1}, SystemText={2}", e.Text, e.ControlText, e.SystemText);

            if (this.currentTerminal != null)
            {
                this.currentTerminal.Send(e.Text);
            }
            //this.textList.HandleTextInput(e);
        }

        #endregion
    }
}