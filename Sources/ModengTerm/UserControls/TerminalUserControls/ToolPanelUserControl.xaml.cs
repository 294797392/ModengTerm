using ModengTerm.Controls;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// ToolPanelUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ToolPanelUserControl : UserControl
    {
        #region 依赖属性

        public ToolPanelVM PanelVM
        {
            get { return (ToolPanelVM)GetValue(PanelVMProperty); }
            set { SetValue(PanelVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelVMProperty =
            DependencyProperty.Register("PanelVM", typeof(ToolPanelVM), typeof(ToolPanelUserControl), new PropertyMetadata(null, PanelVMPropertyChangedCallback));

        #endregion

        #region 构造方法

        public ToolPanelUserControl()
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

        private void OnPanelVMPropertyChanged(ToolPanelVM oldValue, ToolPanelVM newValue)
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
            ToolPanelUserControl me = d as ToolPanelUserControl;
            me.OnPanelVMPropertyChanged(e.OldValue as ToolPanelVM, e.NewValue as ToolPanelVM);
        }

        #endregion

        #region 事件处理器

        private void ListBoxMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToolPanelItemVM selectedItem = ListBoxMenus.SelectedItem as ToolPanelItemVM;
            if (selectedItem == null) 
            {
                return;
            }

            this.PanelVM.InvokeWhenSelectionChanged();
            ContentControl1.Content = this.PanelVM.CurrentContent;
            TextBlockTitle.Text = selectedItem.Name;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (this.PanelVM != null)
            {
                this.PanelVM.Visible = false;
            }
            else
            {
                base.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
