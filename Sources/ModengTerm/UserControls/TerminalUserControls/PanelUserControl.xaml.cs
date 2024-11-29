using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Terminals;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// ToolPanelUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PanelUserControl : UserControl
    {
        #region 依赖属性

        public PanelVM PanelVM
        {
            get { return (PanelVM)GetValue(PanelVMProperty); }
            set { SetValue(PanelVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelVMProperty =
            DependencyProperty.Register("PanelVM", typeof(PanelVM), typeof(PanelUserControl), new PropertyMetadata(null, PanelVMPropertyChangedCallback));

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
            base.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 依赖属性回调

        private void OnPanelVMPropertyChanged(PanelVM oldValue, PanelVM newValue)
        {
            ListBoxMenus.DataContext = newValue;

            Binding binding = new Binding() 
            {
                Source = newValue,
                Path = new PropertyPath("Visible"),
                FallbackValue = Visibility.Collapsed,
                Converter = this.FindResource("BooleanVisibilityConverter") as IValueConverter
            };
            this.SetBinding(VisibilityProperty, binding);
        }

        private static void PanelVMPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PanelUserControl me = d as PanelUserControl;
            me.OnPanelVMPropertyChanged(e.OldValue as PanelVM, e.NewValue as PanelVM);
        }

        #endregion

        #region 事件处理器

        private void ListBoxMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContextMenuVM selectedItem = ListBoxMenus.SelectedItem as ContextMenuVM;
            if (selectedItem == null) 
            {
                return;
            }

            this.PanelVM.SwitchContent(selectedItem);
            ContentControl1.Content = this.PanelVM.CurrentContent;
            TextBlockTitle.Text = selectedItem.Name;
            this.PanelVM.SelectionChangedDelegate(this.PanelVM, e.RemovedItems.Count > 0 ? e.RemovedItems[0] as ContextMenuVM : null, e.AddedItems[0] as ContextMenuVM);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.PanelVM != null)
            {
                this.PanelVM.Visible = false;
                this.PanelVM.CloseDelegate(this.PanelVM);
            }
            else
            {
                base.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
