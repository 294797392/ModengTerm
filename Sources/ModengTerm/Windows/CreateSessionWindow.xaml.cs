using DotNEToolkit;
using ModengTerm;
using ModengTerm.Base.DataModels;
using ModengTerm.Controls;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Session;
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
using XTerminal.Base.Enumerations; 

namespace ModengTerm.Windows
{
    /// <summary>
    /// CreateSessionWindow2.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSessionWindow : MdWindow
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionWindow");

        #region 实例变量

        private Dictionary<Type, Control> contentMap;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前编辑完成了的会话
        /// </summary>
        public XTermSession Session { get; private set; }

        #endregion

        #region 构造方法

        public CreateSessionWindow(SessionGroupVM sessionGroup = null)
        {
            InitializeComponent();

            this.InitializeWindow(sessionGroup);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(SessionGroupVM sessionGroup)
        {
            this.contentMap = new Dictionary<Type, Control>();

            CreateSessionVM createSessionVM = new CreateSessionVM(MTermApp.Context.ServiceAgent);
            base.DataContext = createSessionVM;
            if (sessionGroup != null) 
            {
                createSessionVM.SessionGroups.SelectNode(sessionGroup.ID.ToString());
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
            OptionTreeNodeVM selectedOption = TreeViewOptions.SelectedItem as OptionTreeNodeVM;
            if (selectedOption == null)
            {
                return;
            }

            if (selectedOption.EntryType == null)
            {
                return;
            }

            Control control;
            if (!this.contentMap.TryGetValue(selectedOption.EntryType, out control))
            {
                try
                {
                    control = Activator.CreateInstance(selectedOption.EntryType) as Control;

                    this.contentMap[selectedOption.EntryType] = control;
                }
                catch (Exception ex)
                {
                    logger.Error("创建页面实例异常", ex);
                    return;
                }
            }

            ContentControlContent.Content = control;
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
