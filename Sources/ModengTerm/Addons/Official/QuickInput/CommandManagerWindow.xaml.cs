using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.Addons.QuickInput
{
    /// <summary>
    /// CreateQuickCommandWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CommandManagerWindow : MdWindow
    {
        #region 实例变量

        private List<CommandVM> updateCommands; // 编辑的命令列表
        private List<CommandVM> newCommands;    // 新建的命令列表
        private List<CommandVM> deleteCommands; // 被删除的命令列表
        private BindableCollection<CommandVM> shellCommands;
        private ServiceAgent serviceAgent;
        private ShellSessionVM shellSession;

        #endregion

        #region 属性

        public List<CommandVM> UpdateCommands { get { return this.updateCommands; } }

        public List<CommandVM> NewCommands { get { return this.newCommands; } }

        public List<CommandVM> DeleteCommands { get { return this.deleteCommands; } }

        #endregion

        #region 构造方法

        public CommandManagerWindow(ShellSessionVM shellSession)
        {
            InitializeComponent();

            this.InitializeWindow(shellSession);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(ShellSessionVM shellSession)
        {
            this.serviceAgent = VTApp.Context.ServiceAgent;
            this.shellSession = shellSession;

            this.updateCommands = new List<CommandVM>();
            this.newCommands = new List<CommandVM>();
            this.deleteCommands = new List<CommandVM>();

            ComboBoxCommandTypes.ItemsSource = Enum.GetValues(typeof(CommandTypeEnum));

            List<ShellCommand> commands = this.serviceAgent.GetShellCommands(this.shellSession.ID.ToString());
            this.shellCommands = new BindableCollection<CommandVM>();
            this.shellCommands.AddRange(commands.Select(v => new CommandVM(v)));
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
            CommandVM command = ListBoxShellCommands.SelectedItem as CommandVM;
            if (command != null)
            {
                if (!this.newCommands.Contains(command))
                {
                    this.updateCommands.Add(command);
                }
            }

            foreach (CommandVM cmd in this.newCommands)
            {
                int code = this.serviceAgent.AddShellCommand(cmd.GetShellCommand());
                if (code != ResponseCode.SUCCESS)
                {
                    MTMessageBox.Error("新建命令失败, {0}", code);
                    return;
                }
            }

            foreach (CommandVM cmd in this.deleteCommands)
            {
                int code = this.serviceAgent.DeleteShellCommand(cmd.ID.ToString());
                if (code != ResponseCode.SUCCESS)
                {
                    MTMessageBox.Error("删除命令失败, {0}", code);
                    return;
                }
            }

            foreach (CommandVM cmd in this.updateCommands)
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
                CommandVM command = e.RemovedItems[0] as CommandVM;
                if (!this.newCommands.Contains(command))
                {
                    this.updateCommands.Add(command);
                }
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CommandTypeEnum commandType = CommandTypeEnum.PureText;

            if (ComboBoxCommandTypes.SelectedItem != null) 
            {
                commandType = (CommandTypeEnum)ComboBoxCommandTypes.SelectedItem;
            }

            string name = string.IsNullOrWhiteSpace(TextBoxName.Text) ? "新建命令" : TextBoxName.Text;

            CommandVM command = new CommandVM()
            {
                ID = Guid.NewGuid().ToString(),
                Name = name,
                Type = commandType,
                SessionId = this.shellSession.ID.ToString(),
                Command = TextBoxCommand.Text
            };

            this.shellCommands.Add(command);

            this.newCommands.Add(command);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            CommandVM command = ListBoxShellCommands.SelectedItem as CommandVM;
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
            CommandVM shcmd = ListBoxShellCommands.SelectedItem as CommandVM;
            if (shcmd == null)
            {
                return;
            }

            this.shellCommands.MoveUp(shcmd);
        }

        private void ButtonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            CommandVM shcmd = ListBoxShellCommands.SelectedItem as CommandVM;
            if (shcmd == null)
            {
                return;
            }

            this.shellCommands.MoveDown(shcmd);
        }

        private void ComboBoxCommandTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxCommandTypes.SelectedItem == null)
            {
                return;
            }

            CommandTypeEnum commandType = (CommandTypeEnum)ComboBoxCommandTypes.SelectedItem;

            switch (commandType) 
            {
                case CommandTypeEnum.PureText:
                    {
                        TextBlockTips.Visibility = Visibility.Visible;
                        break;
                    }

                default:
                    {
                        TextBlockTips.Visibility = Visibility.Collapsed;
                        break;
                    }
            }
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            CommandVM command = ListBoxShellCommands.SelectedItem as CommandVM;
            if (command == null) 
            {
                return;
            }

            // InputElement设置为Onwer，Owner和MainWindow有父子关系，那么就可以触发路由事件
            MCommands.SendCommand.Execute(command, this.Owner);
        }

        #endregion
    }
}
