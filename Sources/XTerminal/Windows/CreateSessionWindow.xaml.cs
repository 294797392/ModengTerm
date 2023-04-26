using DotNEToolkit;
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
using System.Windows.Shapes;
using WPFToolkit.MVVM;
using XTerminal.Base;
using XTerminal.Session.Property;
using XTerminal.ViewModels;

namespace XTerminal.Windows
{
    /// <summary>
    /// CreateSessionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSessionWindow : Window
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionWindow");

        #region 实例变量

        private Dictionary<SessionTypeVM, FrameworkElement> stepElementMap;

        #endregion

        #region 构造方法

        public CreateSessionWindow()
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
    }
}
