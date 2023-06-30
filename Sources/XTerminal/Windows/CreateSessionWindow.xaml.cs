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
using XTerminal.Base.DataModels;
using XTerminal.Session.Property;
using XTerminal.Sessions;
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

        private Dictionary<SessionTypeVM, FrameworkElement> propertyContents;

        #endregion

        /// <summary>
        /// 获取当前窗口所编辑的会话
        /// </summary>
        public XTermSession Session { get; private set; }

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
            this.propertyContents = new Dictionary<SessionTypeVM, FrameworkElement>();
            ComboBoxSessionList.SelectedIndex = 0;
        }

        #endregion

        #region 事件处理器

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SessionTypeVM sessionVM = ComboBoxSessionList.SelectedItem as SessionTypeVM;
            if (sessionVM == null)
            {
                return;
            }

            FrameworkElement element;
            if (!this.propertyContents.TryGetValue(sessionVM, out element))
            {
                try
                {
                    element = ConfigFactory<FrameworkElement>.CreateInstance(sessionVM.ProviderEntry);
                    element.DataContext = new XTermSessionVM();
                    this.propertyContents[sessionVM] = element;
                }
                catch (Exception ex)
                {
                    logger.Error("创建Session对应的属性配置页面异常", ex);
                    return;
                }
            }

            ContentControlSessionProperties.Content = element;
            TabItemSessionProperties.IsSelected = true;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            int row, column;
            if (!int.TryParse(TextBoxTerminalRows.Text, out row))
            {
                return;
            }

            if (!int.TryParse(TextBoxTerminalColumns.Text, out column))
            {
                return;
            }

            SessionTypeVM sessionType = ComboBoxSessionList.SelectedItem as SessionTypeVM;
            if (sessionType == null)
            {
                return;
            }

            XTermSessionVM sessionVM = (ContentControlSessionProperties.Content as FrameworkElement).DataContext as XTermSessionVM;

            XTermSession session = new XTermSession()
            {
                ID = sessionVM.ID.ToString(),
                Name = sessionVM.Name,
                Description = sessionVM.Description,
                Row = row,
                Column = column,
                Type = (int)sessionType.Type,
                Host = sessionVM.Host,
                Port = sessionVM.Port,
                Password = sessionVM.Password,
                UserName = sessionVM.UserName,
            };

            this.Session = session;

            base.DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        #endregion
    }
}
