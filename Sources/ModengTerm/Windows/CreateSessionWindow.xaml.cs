using DotNEToolkit;
using ModengTerm;
using ModengTerm.Base.DataModels;
using ModengTerm.Controls;
using ModengTerm.ViewModel.Session;
using ModengTerm.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using ModengTerm.ViewModel.CreateSession;
using ModengTerm.Base;
using ModengTerm.Addon.Controls;

namespace ModengTerm.Windows
{
    /// <summary>
    /// CreateSessionWindow2.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSessionWindow : MdWindow
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionWindow");

        #region 实例变量

        private Dictionary<string, Control> contentMap;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前编辑完成了的会话
        /// </summary>
        public XTermSession Session { get; private set; }

        #endregion

        #region 构造方法

        public CreateSessionWindow(SessionGroupVM selectedGroup = null)
        {
            InitializeComponent();

            this.InitializeWindow(selectedGroup);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(SessionGroupVM selectedGroup)
        {
            this.contentMap = new Dictionary<string, Control>();

            CreateSessionVM createSessionVM = new CreateSessionVM();
            base.DataContext = createSessionVM;
            if (selectedGroup != null)
            {
                createSessionVM.SessionGroups.SelectNode(selectedGroup.ID.ToString());
            }
        }

        #endregion

        #region 事件处理器

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            CreateSessionVM createSessionVM = this.DataContext as CreateSessionVM;

            XTermSession session = createSessionVM.GetSession();
            if (session == null)
            {
                return;
            }

            this.Session = session;

            base.DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void TreeViewOptions_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            PreferenceNodeVM selectedPreference = TreeViewOptions.SelectedItem as PreferenceNodeVM;
            if (selectedPreference == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(selectedPreference.ClassName))
            {
                return;
            }

            if (selectedPreference.FrameworkElement == null)
            {
                FrameworkElement frameworkElement = null;

                try
                {
                    frameworkElement = ConfigFactory<FrameworkElement>.CreateInstance(selectedPreference.ClassName);
                }
                catch (Exception ex)
                {
                    // TODO：显示一个错误页面
                    MTMessageBox.Error("加载预选项页面失败");
                    logger.ErrorFormat(string.Format("创建Preference界面异常, {0}, {1}", selectedPreference.ClassName, ex));
                    return;
                }

                selectedPreference.FrameworkElement = frameworkElement;

                IPreferencePanel preferencePanel = frameworkElement as IPreferencePanel;
                if (preferencePanel != null)
                {
                    preferencePanel.SetOptions(selectedPreference.DefaultOptions);
                }
            }

            ContentControlContent.Content = selectedPreference.FrameworkElement;
        }

        #endregion
    }

    public class SelectedGroupNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string groupName = string.Empty;

            SessionGroupVM current = value as SessionGroupVM;

            while (current != null)
            {
                groupName = string.Format("{0} > ", current.Name) + groupName;

                current = current.Parent as SessionGroupVM;
            }

            if (!string.IsNullOrEmpty(groupName))
            {
                groupName = groupName.Substring(0, groupName.Length - 2);
            }

            return groupName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
