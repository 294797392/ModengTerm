using ModengTerm.ViewModels;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// ToolPanelUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PanelUserControl : UserControl
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("PanelUserControl");

        #region 依赖属性

        public SidePanelVM PanelVM
        {
            get { return (SidePanelVM)GetValue(PanelVMProperty); }
            set { SetValue(PanelVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelVMProperty =
            DependencyProperty.Register("PanelVM", typeof(SidePanelVM), typeof(PanelUserControl), new PropertyMetadata(null, PanelVMPropertyChangedCallback));

        #endregion

        #region 构造方法

        public PanelUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
        }

        private void ProcessContentUnload(SidePanelItemVM panelItemVM)
        {
            if (!(panelItemVM.ContentVM is MenuContentVM))
            {
                return;
            }

            MenuContentVM menuContentVM = panelItemVM.ContentVM as MenuContentVM;
            menuContentVM.OnUnload();
        }

        #endregion

        #region 依赖属性回调

        private void OnPanelVMPropertyChanged(SidePanelVM oldValue, SidePanelVM newValue)
        {
            ListBoxMenus.DataContext = newValue;
        }

        private static void PanelVMPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PanelUserControl me = d as PanelUserControl;
            me.OnPanelVMPropertyChanged(e.OldValue as SidePanelVM, e.NewValue as SidePanelVM);
        }

        #endregion

        #region 事件处理器

        private void ListBoxMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SidePanelItemVM selectedItem = ListBoxMenus.SelectedItem as SidePanelItemVM;
            if (selectedItem == null)
            {
                return;
            }

            DependencyObject dependencyObject = this.PanelVM.LoadContent(selectedItem);
            if (dependencyObject == null)
            {
                logger.ErrorFormat("加载页面失败, 页面为空, {0}", selectedItem.Name);
                return;
            }

            ContentControl1.Content = dependencyObject;
            TextBlockTitle.Text = selectedItem.Name;
            GridContent.SetCurrentValue(Grid.VisibilityProperty, Visibility.Visible);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            SidePanelItemVM panelItemVM = ListBoxMenus.SelectedItem as SidePanelItemVM;

            ListBoxMenus.SelectedItem = null;
            GridContent.SetCurrentValue(Grid.VisibilityProperty, Visibility.Collapsed);

            // 点击关闭按钮手动触发Unload事件
            this.ProcessContentUnload(panelItemVM);
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SidePanelItemVM selected = ListBoxMenus.SelectedItem as SidePanelItemVM;

            SidePanelItemVM clicked = (sender as ListBoxItem).DataContext as SidePanelItemVM;

            if (clicked == selected)
            {
                // 收回
                GridContent.SetCurrentValue(Grid.VisibilityProperty, Visibility.Collapsed);
                ListBoxMenus.SelectedItem = null;

                this.ProcessContentUnload(clicked);

                // 在执行点击动作之前阻止执行点击动作，就不会再次执行点击的动作了
                e.Handled = true;
            }
        }

        #endregion
    }
}
