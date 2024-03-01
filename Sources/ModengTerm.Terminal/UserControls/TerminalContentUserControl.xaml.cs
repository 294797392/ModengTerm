using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Rendering;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.Utility;

namespace ModengTerm.Terminal.UserControls
{
    /// <summary>
    /// TerminalContentUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalContentUserControl : UserControl, ISessionContent
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #endregion

        #region 实例变量

        private ShellSessionVM shellSession;
        private IVideoTerminal videoTerminal;

        #endregion

        #region 属性

        public XTermSession Session { get; set; }

        #endregion

        #region 构造方法

        public TerminalContentUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
#if !DEBUG
            ToggleButtonLoggerSetting.Visibility = Visibility.Collapsed;
#endif

            this.Background = Brushes.Transparent;
        }

        /// <summary>
        /// 获取当前正在显示的文档的事件输入器
        /// </summary>
        /// <returns></returns>
        private VTEventInput GetActiveEventInput()
        {
            VTDocument activeDocument = this.videoTerminal.ActiveDocument;
            return activeDocument.EventInput;
        }

        #endregion

        #region 事件处理器

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();

            // 右击如果设置了e.Handled = true的话会阻止右键菜单的弹出
            if (e.ChangedButton == MouseButton.Right)
            {
                return;
            }

            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseDown(e.GetPosition(GridDocument).ToVTPoint(), e.ClickCount);

            // 阻止事件继续传播
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseMove(e.GetPosition(GridDocument).ToVTPoint());

            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Right)
            {
                return;
            }

            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseUp(e.GetPosition(GridDocument).ToVTPoint());

            e.Handled = true;
        }

        /// <summary>
        /// 重写了这个事件后，就会触发鼠标相关的事件
        /// </summary>
        /// 参考AvalonEdit
        /// <param name="hitTestParameters"></param>
        /// <returns></returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            ShellFunctionMenu functionMenu = menuItem.DataContext as ShellFunctionMenu;
            if (functionMenu == null)
            {
                return;
            }

            if (functionMenu.Execute == null)
            {
                return;
            }

            functionMenu.Execute();
        }

        #endregion

        #region ISessionContent

        public int Open()
        {
            this.shellSession = this.DataContext as ShellSessionVM;
            this.shellSession.MainDocument = DocumentMain;
            this.shellSession.AlternateDocument = DocumentAlternate;
            this.shellSession.Open();

            this.videoTerminal = this.shellSession.VideoTerminal;

            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            this.shellSession.Close();
            this.videoTerminal = null;
        }

        #endregion
    }
}
