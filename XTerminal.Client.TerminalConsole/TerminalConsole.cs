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
using XTerminal.Client.TerminalConsole.Rendering;
using XTerminal.Terminal;

namespace XTerminal.Client.TerminalConsole
{
    public class TerminalConsole : Control
    {
        #region 实例变量

        private Grid grid;
        private TerminalTextList textPanel;
        private CaretLayer caretLayer;

        #endregion

        #region 属性

        public static readonly DependencyProperty TerminalEngineProperty = DependencyProperty.Register("TerminalEngine", typeof(ITerminal), typeof(TerminalConsole), new PropertyMetadata(null, new PropertyChangedCallback(TerminalEnginePropertyChangedCallback)));
        public ITerminal TerminalEngine
        {
            get
            {
                return base.GetValue(TerminalEngineProperty) as ITerminal;
            }
            set
            {
                base.SetValue(TerminalEngineProperty, value);
            }
        }

        #endregion

        #region 构造方法

        public TerminalConsole()
        {
            this.InitializeConsole();
        }

        #endregion

        #region 依赖属性回调

        private static void TerminalEnginePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TerminalConsole).OnTerminalEnginePropertyChanged(e.NewValue, e.OldValue);
        }

        protected void OnTerminalEnginePropertyChanged(object newEngine, object oldEngine)
        {
            if (oldEngine != null)
            {
                this.ReleaseTerminalEngine(oldEngine as ITerminal);
            }

            if (newEngine != null)
            {
                this.InitializeTerminalEngine(newEngine as ITerminal);
            }
        }

        #endregion

        #region 实例方法

        private void InitializeConsole()
        {
            this.textPanel = new TerminalTextList();
            this.caretLayer = new CaretLayer();
        }

        private void InitializeTerminalEngine(ITerminal terminal)
        {
            terminal.CommandReceived += Terminal_CommandReceived;
            terminal.Initialize();
            terminal.Connect();
        }

        private void ReleaseTerminalEngine(ITerminal terminal)
        {
            terminal.Disconnect();
            terminal.Release();
            terminal.CommandReceived -= Terminal_CommandReceived;
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.grid = base.Template.FindName("PART_Grid", this) as Grid;
            if (this.grid != null)
            {
                this.grid.Children.Add(this.caretLayer);
            }
        }

        #endregion

        #region 事件处理器

        private void Terminal_CommandReceived(object sender, IEnumerable<IEscapeSequencesCommand> cmds)
        {

        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            Console.WriteLine("Text={0}, ControlText={1}, SystemText={2}", e.Text, e.ControlText, e.SystemText);

            this.textPanel.HandleTextInput(e);
        }

        #endregion
    }
}