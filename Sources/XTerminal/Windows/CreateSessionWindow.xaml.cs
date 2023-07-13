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
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Session.Definitions;
using XTerminal.Session.Enumerations;
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
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionWindow");

        #endregion

        #region 实例变量

        private BindableCollection<SessionTypeVM> sessionTypeList;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前窗口所编辑的会话
        /// </summary>
        public XTermSession Session { get; private set; }

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
            // 初始化SSH验证方式列表
            ComboBoxAuthList.ItemsSource = Enum.GetValues(typeof(SSHAuthTypeEnum));
            ComboBoxAuthList.SelectedIndex = 0;

            // 初始化会话列表
            this.sessionTypeList = new BindableCollection<SessionTypeVM>();
            List<SessionDefinition> sessions = XTermApp.Context.ServiceAgent.GetSessionDefinitions();
            foreach (SessionDefinition session in sessions)
            {
                this.sessionTypeList.Add(new SessionTypeVM(session));
            }
            ComboBoxSessionTypes.ItemsSource = this.sessionTypeList;
            ComboBoxSessionTypes.SelectedIndex = 0;
        }

        #endregion

        #region 事件处理器

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ComboBoxAuthList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SSHAuthTypeEnum authType = (SSHAuthTypeEnum)ComboBoxAuthList.SelectedItem;

            switch (authType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionUserName.Height = new GridLength(0);
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        RowDefinitionPassword.Height = new GridLength(35);
                        RowDefinitionUserName.Height = new GridLength(35);
                        break;
                    }

                case SSHAuthTypeEnum.PulicKey:
                    {
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionUserName.Height = new GridLength(0);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
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

            string sessionName = TextBoxSessionName.Text;
            if (string.IsNullOrEmpty(sessionName))
            {
                MessageBoxUtils.Info("请输入会话名称");
                return;
            }

            SessionTypeVM sessionType = ComboBoxSessionTypes.SelectedItem as SessionTypeVM;
            if (sessionType == null)
            {
                return;
            }

            switch (sessionType.Type)
            {
                case SessionTypeEnum.SSH:
                    {
                        string hostName = TextBoxSSHHostName.Text;
                        if (string.IsNullOrEmpty(hostName))
                        {
                            MessageBoxUtils.Info("请输入主机名称");
                            return;
                        }

                        int port;
                        if (!int.TryParse(TextBoxSSHPort.Text, out port) ||
                            port < XTermConsts.MIN_PORT || port > XTermConsts.MAX_PORT)
                        {
                            MessageBoxUtils.Info("请输入正确的端口号");
                            return;
                        }

                        if (ComboBoxAuthList.SelectedItem == null)
                        {
                            MessageBoxUtils.Info("请选择身份验证方式");
                            return;
                        }

                        string password = string.Empty;
                        string userName = string.Empty;

                        SSHAuthTypeEnum authType = (SSHAuthTypeEnum)ComboBoxAuthList.SelectedItem;
                        switch (authType)
                        {
                            case SSHAuthTypeEnum.None:
                                {
                                    break;
                                }

                            case SSHAuthTypeEnum.Password:
                                {
                                    userName = TextBoxSSHUserName.Text;
                                    if (string.IsNullOrEmpty(userName))
                                    {
                                        MessageBoxUtils.Info("请输入用户名");
                                        return;
                                    }

                                    password = PasswordBoxSSHPassword.Password;
                                    if (string.IsNullOrEmpty(password))
                                    {
                                        MessageBoxUtils.Info("请输入密码");
                                        return;
                                    }

                                    break;
                                }

                            default:
                                throw new NotImplementedException();
                        }

                        XTermSession session = new XTermSession()
                        {
                            ID = Guid.NewGuid().ToString(),
                            Name = TextBoxSessionName.Text,
                            CreationTime = DateTime.Now,
                            Row = row,
                            Column = column,
                            Type = (int)sessionType.Type,
                            AuthType = (int)authType,
                            Host = hostName,
                            Password = password,
                            Port = port,
                            UserName = userName,
                        };
                        this.Session = session;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            base.DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        #endregion
    }
}
