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

        private Dock dock = Dock.Left;

        public Dock Dock
        {
            get { return this.dock; }
            set
            {
                if (this.dock != value)
                {
                    this.dock = value;

                    switch (value)
                    {
                        case Dock.Right:
                            {
                                Grid.SetColumn(ListBoxMenus, 1);
                                Grid.SetColumn(GridContent, 0);
                                ColumnDefinition1.Width = new GridLength();
                                ColumnDefinition2.Width = new GridLength(0, GridUnitType.Auto);
                                ListBoxMenus.ItemContainerStyle = this.FindResource("StyleListBoxItemSessionPanel") as Style;
                                break;
                            }

                        default:
                            throw new System.NotImplementedException();
                    }
                }
            }
        }

        #region 依赖属性

        //public PanelVM PanelVM
        //{
        //    get { return (PanelVM)GetValue(PanelVMProperty); }
        //    set { SetValue(PanelVMProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for PanelVM.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PanelVMProperty =
        //    DependencyProperty.Register("PanelVM", typeof(PanelVM), typeof(PanelUserControl), new PropertyMetadata(null, PanelVMPropertyChangedCallback));

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

        private void ProcessContentUnload(PanelItemVM panelItemVM)
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

        //private void OnPanelVMPropertyChanged(PanelVM oldValue, PanelVM newValue)
        //{
        //    ListBoxMenus.DataContext = newValue;
        //}

        //private static void PanelVMPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    PanelUserControl me = d as PanelUserControl;
        //    me.OnPanelVMPropertyChanged(e.OldValue as PanelVM, e.NewValue as PanelVM);
        //}

        #endregion

        #region 事件处理器

        private void ListBoxMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (base.DataContext == null) 
            {
                return;
            }

            PanelVM panelVM = base.DataContext as PanelVM;

            PanelItemVM selectedItem = ListBoxMenus.SelectedItem as PanelItemVM;
            if (selectedItem == null)
            {
                panelVM.CurrentContent = null;
                return;
            }

            DependencyObject dependencyObject = panelVM.LoadContent(selectedItem);
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
            PanelItemVM panelItemVM = ListBoxMenus.SelectedItem as PanelItemVM;

            ListBoxMenus.SelectedItem = null;
            GridContent.SetCurrentValue(Grid.VisibilityProperty, Visibility.Collapsed);

            // 点击关闭按钮手动触发Unload事件
            this.ProcessContentUnload(panelItemVM);
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PanelItemVM selected = ListBoxMenus.SelectedItem as PanelItemVM;

            PanelItemVM clicked = (sender as ListBoxItem).DataContext as PanelItemVM;

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
