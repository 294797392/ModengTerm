using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.ServiceAgents;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Windows.Terminals
{
    /// <summary>
    /// CreateQuickCommandWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateShellCommandWindow : Window
    {
        private static readonly string[] Splitter = new string[] { " " };

        private ServiceAgent serviceAgent;

        #region 属性

        /// <summary>
        /// 新创建的ShellCommand
        /// </summary>
        public ShellCommand Command { get; private set; }

        #endregion

        public CreateShellCommandWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        private void InitializeWindow()
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;

            ComboBoxCommandTypes.ItemsSource = Enum.GetValues(typeof(CommandTypeEnum));
            ComboBoxCommandTypes.SelectedIndex = 0;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBoxName.Text;
            if (string.IsNullOrEmpty(name))
            {
                MTMessageBox.Info("请输入命令名称");
                return;
            }

            string command = TextBoxCommand.Text;
            if (string.IsNullOrEmpty(command))
            {
                MTMessageBox.Info("请输入命令内容");
                return;
            }

            CommandTypeEnum commandType = (CommandTypeEnum)ComboBoxCommandTypes.SelectedItem;
            switch (commandType)
            {
                case CommandTypeEnum.HexData:
                    {
                        byte[] bytes;
                        if (!MTermUtils.TryParseHexString(command, out bytes))
                        {
                            MTMessageBox.Info("请输入正确的十六进制数据");
                            return;
                        }

                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            this.Command = new ShellCommand();
            this.Command.ID = Guid.NewGuid().ToString();
            this.Command.Name = TextBoxName.Text;
            this.Command.Command = TextBoxCommand.Text;
            this.Command.Type = (int)commandType;

            int code = this.serviceAgent.AddShellCommand(this.Command);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("新建命令失败, {0}", code);
                return;
            }

            base.DialogResult = true;
        }
    }
}

