using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.ServiceAgents;
using ModengTerm.ViewModels.Terminals;
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
using WPFToolkit.MVVM;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Windows.Terminals
{
    /// <summary>
    /// CreateQuickCommandWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateShellCommandWindow : Window
    {
        #region 实例变量

        private List<ShellCommandVM> updateCommands; // 编辑的命令列表
        private List<ShellCommandVM> newCommands;    // 新建的命令列表
        private List<ShellCommandVM> deleteCommands; // 被删除的命令列表
        private BindableCollection<ShellCommandVM> shellCommands;
        private ServiceAgent serviceAgent;

        #endregion

        #region 属性

        public List<ShellCommandVM> UpdateCommands { get { return this.updateCommands; } }

        public List<ShellCommandVM> NewCommands { get { return this.newCommands; } }

        public List<ShellCommandVM> DeleteCommands { get { return this.deleteCommands; } }

        #endregion

        #region 构造方法

        public CreateShellCommandWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;

            this.updateCommands = new List<ShellCommandVM>();
            this.newCommands = new List<ShellCommandVM>();
            this.deleteCommands = new List<ShellCommandVM>();

            ComboBoxCommandTypes.ItemsSource = Enum.GetValues(typeof(CommandTypeEnum));
            ComboBoxCommandTypes.SelectedIndex = 0;

            List<ShellCommand> commands = this.serviceAgent.GetShellCommands();
            this.shellCommands = new BindableCollection<ShellCommandVM>();
            this.shellCommands.AddRange(commands.Select(v => new ShellCommandVM(v)));
            ListBoxShellCommands.DataContext = this.shellCommands;
        }

        #endregion

        #region 事件处理器

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // 判断是否编辑了命令
            ShellCommandVM command = ListBoxShellCommands.SelectedItem as ShellCommandVM;
            if (command != null)
            {
                if (!this.newCommands.Contains(command))
                {
                    this.updateCommands.Add(command);
                }
            }

            foreach (ShellCommandVM cmd in this.newCommands)
            {
                int code = this.serviceAgent.AddShellCommand(cmd.GetShellCommand());
                if (code != ResponseCode.SUCCESS)
                {
                    MTMessageBox.Error("新建命令失败, {0}", code);
                    return;
                }
            }

            foreach (ShellCommandVM cmd in this.deleteCommands)
            {
                int code = this.serviceAgent.DeleteShellCommand(cmd.ID.ToString());
                if (code != ResponseCode.SUCCESS)
                {
                    MTMessageBox.Error("删除命令失败, {0}", code);
                    return;
                }
            }

            foreach (ShellCommandVM cmd in this.updateCommands)
            {
                int code = this.serviceAgent.UpdateShellCommand(cmd.GetShellCommand());
                if (code != ResponseCode.SUCCESS)
                {
                    MTMessageBox.Error("更新命令失败, {0}", code);
                    return;
                }
            }

            base.DialogResult = true;
        }

        private void ListBoxShellCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                ShellCommandVM command = e.RemovedItems[0] as ShellCommandVM;
                if (!this.newCommands.Contains(command)) 
                {
                    this.updateCommands.Add(command);
                }
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            ShellCommandVM command = new ShellCommandVM()
            {
                ID = Guid.NewGuid().ToString(),
                Name = "新建命令",
                Type = (int)CommandTypeEnum.PureText
            };

            this.shellCommands.Add(command);

            this.newCommands.Add(command);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ShellCommandVM command = ListBoxShellCommands.SelectedItem as ShellCommandVM;
            if (command == null) 
            {
                return;
            }

            if (!MTMessageBox.Confirm("确定要删除{0}吗?", command.Name))
            {
                return;
            }

            this.shellCommands.Remove(command);

            if (this.newCommands.Contains(command))
            {
                this.newCommands.Remove(command);
            }
            else
            {
                this.deleteCommands.Add(command);
                this.updateCommands.Remove(command);
            }
        }

        private void ButtonMoveUp_Click(object sender, RoutedEventArgs e)
        {
            ShellCommandVM shcmd = ListBoxShellCommands.SelectedItem as ShellCommandVM;
            if (shcmd == null) 
            {
                return;
            }

            this.shellCommands.MoveUp(shcmd);
        }

        private void ButtonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            ShellCommandVM shcmd = ListBoxShellCommands.SelectedItem as ShellCommandVM;
            if (shcmd == null)
            {
                return;
            }

            this.shellCommands.MoveDown(shcmd);
        }

        #endregion
    }
}

