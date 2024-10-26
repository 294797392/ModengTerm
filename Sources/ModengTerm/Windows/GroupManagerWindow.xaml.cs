using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using ModengTerm.ViewModels.Session;
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

namespace ModengTerm.Windows
{
    /// <summary>
    /// CreateSessionGroupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GroupManagerWindow : MdWindow
    {
        public event Action<SessionGroup> OnSessionGroupDeleted;
        public event Action<SessionGroup> OnSessionGroupCreated;

        private SessionTreeVM sessionTreeVM;
        private ServiceAgent serviceAgent;
        private string keyword;

        public GroupManagerWindow(string selectedGroupId = null)
        {
            InitializeComponent();

            this.InitializeWindow(selectedGroupId);
        }

        private void InitializeWindow(string selectedGroupId)
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;
            this.sessionTreeVM = MTermApp.Context.CreateSessionTreeVM(false, true);
            SessionTreeViewUserControl.ViewModel = this.sessionTreeVM;
            this.sessionTreeVM.ExpandAll();

            if (!string.IsNullOrEmpty(selectedGroupId))
            {
                this.sessionTreeVM.SelectNode(selectedGroupId);
                this.sessionTreeVM.ExpandNode(selectedGroupId);
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MTMessageBox.Info("请输入正确的分组名");
                return;
            }

            SessionGroupVM parentGroup = this.sessionTreeVM.Context.SelectedItem as SessionGroupVM;
            if (parentGroup == null)
            {
                parentGroup = this.sessionTreeVM.Roots[0] as SessionGroupVM;
            }

            SessionGroup sessionGroup = new SessionGroup()
            {
                ID = Guid.NewGuid().ToString(),
                ParentId = parentGroup.ID.ToString(),
                Name = TextBoxName.Text,
            };

            int code = this.serviceAgent.AddSessionGroup(sessionGroup);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("新建分组失败, {0}", code);
                return;
            }

            SessionGroupVM sessionGroupVM = new SessionGroupVM(this.sessionTreeVM.Context, sessionGroup);
            parentGroup.Add(sessionGroupVM);

            parentGroup.IsExpanded = true;

            this.OnSessionGroupCreated?.Invoke(sessionGroup);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = true;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                return;
            }

            if (this.keyword != TextBoxName.Text)
            {
                this.keyword = TextBoxName.Text;
                this.sessionTreeVM.Keyword = keyword;
                this.sessionTreeVM.ResetMatch();
            }

            TreeNodeViewModel matches;
            this.sessionTreeVM.PerformMatch(out matches);

            // TODO：滚动条滚动到匹配的节点位置
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            SessionGroupVM sessionGroupVM = this.sessionTreeVM.DeleteSelectedGroup();
            if (sessionGroupVM == null)
            {
                return;
            }

            this.OnSessionGroupDeleted?.Invoke(sessionGroupVM.Data as SessionGroup);
        }
    }
}
