using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using XTerminal.Session.Property;
using XTerminal.ViewModels;

namespace XTerminal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MainWindow");

        #endregion

        #region 实例变量

        private Dictionary<SessionTypeVM, FrameworkElement> stepElementMap;

        #endregion

        #region 构造方法

        public MainWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            this.stepElementMap = new Dictionary<SessionTypeVM, FrameworkElement>();
        }

        #endregion

        #region 事件处理器

        private void ListBoxSessionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SessionTypeVM sessionVM = ListBoxSessionList.SelectedItem as SessionTypeVM;
            if (sessionVM == null)
            {
                return;
            }

            FrameworkElement element;
            if (!this.stepElementMap.TryGetValue(sessionVM, out element))
            {
                try
                {
                    element = ConfigFactory<FrameworkElement>.CreateInstance(sessionVM.ProviderEntry);
                    element.DataContext = SessionPropertiesVM.Create(sessionVM.Type);
                    this.stepElementMap[sessionVM] = element;
                }
                catch (Exception ex)
                {
                    logger.Error("创建Session对应的属性配置页面异常", ex);
                    return;
                }
            }

            GridStep1.Visibility = Visibility.Collapsed;
            GridStep2.Visibility = Visibility.Visible;
            ContentControlSessionProperties.Content = element;
            TabItemSessionProperties.IsSelected = true;
            TabItemSessionProperties.DataContext = element.DataContext;
        }

        private void ButtonCompleted_Click(object sender, RoutedEventArgs e)
        {
            SessionPropertiesVM sessionPropertiesVM = TabItemSessionProperties.DataContext as SessionPropertiesVM;
            SessionProperties sessionProperties = sessionPropertiesVM.GetProperties();
        }

        #endregion
    }
}
