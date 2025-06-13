using DotNEToolkit;
using ModengTerm.Addon.Extensions;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel;
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

        private FrameworkElement LoadContent(SidePanel panel)
        {
            PanelDefinition definition = panel.Definition;
            FrameworkElement content = panel.Content;
            SidePanelExtension extensionObject = panel.ExtensionObject;

            // 先创建扩展对象
            if (extensionObject == null)
            {
                try
                {
                    extensionObject = ConfigFactory<SidePanelExtension>.CreateInstance(definition.VMClassName);
                    panel.ExtensionObject = extensionObject;
                    extensionObject.Name = panel.Name;
                }
                catch (Exception ex)
                {
                    logger.Error("创建扩展SidePanel对象异常", ex);
                    return null;
                }
            }

            // 开始加载本次选中的菜单界面
            if (content == null)
            {
                try
                {
                    content = ConfigFactory<FrameworkElement>.CreateInstance(definition.ClassName);

                    panel.Initialize();
                    panel.Content = content;

                    content.DataContext = extensionObject;
                }
                catch (Exception ex)
                {
                    logger.Error("创建菜单内容控件异常", ex);
                    return null;
                }
            }

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

            if (e.RemovedItems.Count > 0)
            {
                SidePanel panel = e.RemovedItems[0] as SidePanel;
                panel.Unloaded();
            }

            PanelContainer panelContainer = base.DataContext as PanelContainer;

            SidePanel selectedItem = ListBoxMenus.SelectedItem as SidePanel;
            if (selectedItem == null)
            {
                GridContent.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
                ContentControl1.Content = null;
                return;
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

            if (e.AddedItems.Count > 0) 
            {
                SidePanel panel = e.AddedItems[0] as SidePanel;
                panel.Loaded();
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SidePanel selected = ListBoxMenus.SelectedItem as SidePanel;

            SidePanel clicked = (sender as ListBoxItem).DataContext as SidePanel;

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
            SidePanel selectedPanel = ListBoxMenus.SelectedItem as SidePanel;

            ListBoxMenus.SelectedItem = null;
            ContentControl1.Content = null;
            GridContent.SetValue(Grid.VisibilityProperty, Visibility.Collapsed);
        }

        #endregion
    }
}
