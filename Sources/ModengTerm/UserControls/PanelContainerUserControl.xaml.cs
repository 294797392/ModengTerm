using DotNEToolkit;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Panels;
using Renci.SshNet;
using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace ModengTerm.UserControls
{
    /// <summary>
    /// PanelContainerUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PanelContainerUserControl : UserControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PanelContainerUserControl");

        #endregion

        #region 实例变量

        private Dock dock = Dock.Left;

        #endregion

        #region 属性

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
                                ListBoxMenus.ItemContainerStyle = this.FindResource("StyleListBoxItemRightDock") as Style;
                                ListBoxMenus.BorderThickness = new Thickness(1, 0, 0, 0);
                                BorderContent.BorderThickness = new Thickness(1, 0, 0, 0);
                                break;
                            }

                        default:
                            throw new System.NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region 构造方法

        public PanelContainerUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
        }

        #endregion

        #region 事件处理器

        private void ListBoxMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (base.DataContext == null) 
            {
                return;
            }

            if (e.RemovedItems.Count > 0)
            {
                SidePanelVM panel = e.RemovedItems[0] as SidePanelVM;
                panel.Unloaded();
            }

            PanelContainer panelContainer = base.DataContext as PanelContainer;

            SidePanelVM selectedItem = ListBoxMenus.SelectedItem as SidePanelVM;
            if (selectedItem == null)
            {
                GridContent.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
                ContentControl1.Content = null;
                return;
            }

            DependencyObject dependencyObject = selectedItem.Panel;
            if (dependencyObject == null)
            {
                logger.ErrorFormat("加载页面失败, 页面为空, {0}", selectedItem.Name);
                return;
            }

            ContentControl1.Content = dependencyObject;
            TextBlockTitle.Text = selectedItem.Name;

            GridContent.SetValue(Grid.VisibilityProperty, Visibility.Visible);

            if (e.AddedItems.Count > 0) 
            {
                SidePanelVM panel = e.AddedItems[0] as SidePanelVM;
                panel.Loaded();
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SidePanelVM selected = ListBoxMenus.SelectedItem as SidePanelVM;

            SidePanelVM clicked = (sender as ListBoxItem).DataContext as SidePanelVM;

            if (clicked == selected)
            {
                // 触发SelectionChanged事件，SelectionChanged事件里隐藏和触发OnUnload事件
                ListBoxMenus.SelectedItem = null;

                // 不继续触发SelectionChanged事件
                e.Handled = true;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            SidePanelVM selectedPanel = ListBoxMenus.SelectedItem as SidePanelVM;

            ListBoxMenus.SelectedItem = null;
            ContentControl1.Content = null;
            GridContent.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
        }

        #endregion
    }
}
