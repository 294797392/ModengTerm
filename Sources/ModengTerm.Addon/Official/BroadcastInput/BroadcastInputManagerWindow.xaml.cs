using ModengTerm.Addons;
using ModengTerm.Addons.Shell;
using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Official.BroadcastInput
{
    /// <summary>
    /// SendAllConfigurationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BroadcastInputManagerWindow : MdWindow
    {
        #region 实例变量

        private BindableCollection<BroadcastSessionVM> broadcastSessions;

        #endregion

        #region 实例变量

        /// <summary>
        /// 被删除的广播会话
        /// </summary>
        private List<BroadcastSessionVM> removeList;

        /// <summary>
        /// 新增加的广播会话
        /// </summary>
        private List<BroadcastSessionVM> addList;

        #endregion

        #region 属性

        public AddonObjectStorage ObjectStorage { get; set; }

        public string SessionId { get; set; }

        #endregion

        #region 构造方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broadcastSessions"></param>
        /// <param name="shellSessions">当前打开的所有终端类型的会话</param>
        public BroadcastInputManagerWindow(List<BroadcastSessionVM> broadcastSessions, List<ITerminalShell> shellSessions)
        {
            InitializeComponent();

            this.InitializeWindow(broadcastSessions, shellSessions);
        }

        #endregion

        #region 实例方法

        private void InitializeWindow(List<BroadcastSessionVM> broadcastSessions, List<ITerminalShell> shellSessions)
        {
            this.broadcastSessions = new BindableCollection<BroadcastSessionVM>();
            this.broadcastSessions.AddRange(broadcastSessions);

            ListBoxShellSessionList.DataContext = shellSessions;
            ListBoxBroadcastSessions.DataContext = this.broadcastSessions;
        }

        #endregion

        #region 事件处理器

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<BroadcastSession> addList = this.addList.Select(v => new BroadcastSession() { SessionId = v.Session.Id }).ToList();
            int code = this.ObjectStorage.AddObjects<BroadcastSession>(this.SessionId, addList);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("新建广播会话失败, {0}", code);
                return;
            }

            IEnumerable<string> removeList = this.removeList.Select(v => v.Session.Id);
            code = this.ObjectStorage.DeleteObjects(this.SessionId, removeList);
            if (code != ResponseCode.SUCCESS) 
            {
                MTMessageBox.Error("删除广播会话失败, {0}", code);
                return;
            }

            base.DialogResult = true;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonAddSlave_Click(object sender, RoutedEventArgs e)
        {
            BroadcastSessionVM broadcastSession = ListBoxShellSessionList.SelectedItem as BroadcastSessionVM;
            if (broadcastSession == null)
            {
                return;
            }

            if (this.broadcastSessions.Contains(broadcastSession))
            {
                return;
            }

            if (this.removeList.Contains(broadcastSession))
            {
                this.removeList.Remove(broadcastSession);
            }

            this.addList.Add(broadcastSession);
            this.broadcastSessions.Add(broadcastSession);
        }

        private void ButtonRemoveSlave_Click(object sender, RoutedEventArgs e)
        {
            BroadcastSessionVM broadcastSession = ListBoxShellSessionList.SelectedItem as BroadcastSessionVM;
            if (broadcastSession == null)
            {
                return;
            }

            if (!this.broadcastSessions.Contains(broadcastSession))
            {
                return;
            }

            if (this.addList.Contains(broadcastSession))
            {
                this.addList.Remove(broadcastSession);
            }

            this.removeList.Add(broadcastSession);
            this.broadcastSessions.Remove(broadcastSession);
        }

        private void MenuItemClearSelected_Click(object sender, RoutedEventArgs e)
        {
            this.removeList.Clear();
            this.addList.Clear();

            this.removeList.AddRange(this.broadcastSessions);
            this.broadcastSessions.Clear();
        }

        #endregion
    }
}
