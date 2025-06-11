using DotNEToolkit;
using ModengTerm.Base.Addon.ViewModel;
using ModengTerm.Base.Definitions;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using WPFToolkit.MVVM;

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
                                ListBoxMenus.ItemContainerStyle = this.FindResource("StyleListBoxItemSessionPanel") as Style;
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

        private void ProcessContentUnload(PanelBase panel)
        {
            panel.OnUnload();
        }

        private FrameworkElement LoadContent(PanelBase panel)
        {
            PanelDefinition definition = panel.Definition;
            FrameworkElement content = panel.Content;

            // 开始加载本次选中的菜单界面
            if (content == null)
            {
                try
                {
                    content = ConfigFactory<FrameworkElement>.CreateInstance(definition.ClassName);

                    panel.OnInitialize();
                    panel.Content = content;

                    content.DataContext = panel;
                }
                catch (Exception ex)
                {
                    logger.Error("创建菜单内容控件异常", ex);
                    return null;
                }
            }

            panel.OnLoaded();

            return content;
        }

        #endregion

        #region 事件处理器

        private void ListBoxMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (base.DataContext == null) 
            {
                return;
            }

            PanelContainer panelContainer = base.DataContext as PanelContainer;

            PanelBase selectedItem = ListBoxMenus.SelectedItem as PanelBase;
            if (selectedItem == null)
            {
                GridContent.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
                ContentControl1.Content = null;
                return;
            }

            if (e.RemovedItems.Count > 0) 
            {
                this.ProcessContentUnload(e.RemovedItems[0] as PanelBase);
            }

            DependencyObject dependencyObject = this.LoadContent(selectedItem);
            if (dependencyObject == null)
            {
                logger.ErrorFormat("加载页面失败, 页面为空, {0}", selectedItem.Name);
                return;
            }

            ContentControl1.Content = dependencyObject;
            TextBlockTitle.Text = selectedItem.Name;

            GridContent.SetValue(Grid.VisibilityProperty, Visibility.Visible);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            PanelBase selectedPanel = ListBoxMenus.SelectedItem as PanelBase;

            ListBoxMenus.SelectedItem = null;
            ContentControl1.Content = null;
            GridContent.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);

            // 点击关闭按钮手动触发Unload事件
            this.ProcessContentUnload(selectedPanel);
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PanelBase selected = ListBoxMenus.SelectedItem as PanelBase;

            PanelBase clicked = (sender as ListBoxItem).DataContext as PanelBase;

            if (clicked == selected)
            {
                // 收回
                GridContent.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
                ListBoxMenus.SelectedItem = null;

                this.ProcessContentUnload(clicked);

                // 在执行点击动作之前阻止执行点击动作，就不会再次执行点击的动作了
                e.Handled = true;
            }
        }

        #endregion
    }
}
