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
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XTerminal.Client.Controls.TerminalConsole"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XTerminal.Client.Controls.TerminalConsole;assembly=XTerminal.Client.Controls.TerminalConsole"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// 1.选中单词：通过空格去判断一个完整的单词并选中
    /// 
    /// </summary>
    public class TerminalConsole : Control
    {
        #region 实例变量

        private Grid grid;
        private ScrollViewer scrollViewer;
        private TerminalTextPanel textPanel;
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
            this.textPanel = new TerminalTextPanel();
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

            this.scrollViewer = base.Template.FindName("PART_ScrollViewer", this) as ScrollViewer;
            if (this.scrollViewer != null)
            {
                scrollViewer.Content = this.textPanel;
                scrollViewer.CanContentScroll = true;
            }

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