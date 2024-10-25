using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using ModengTerm.UserControls.SessionListUserControls;
using ModengTerm.ViewModels.Session;
using ModengTerm.ViewModels.Sessions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;
using XTerminal.Windows;

namespace ModengTerm.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SessionListWindow : MdWindow
    {
        public enum SessionListViewEnum
        {
            /// <summary>
            /// 普通DataGrid风格
            /// </summary>
            DataGrid,

            /// <summary>
            /// XShell DataList风格
            /// </summary>
            DataList,

            ///// <summary>
            ///// Win10任务管理器风格
            ///// </summary>
            //TreeList,

            ///// <summary>
            ///// termius风格
            ///// </summary>
            //GroupDataList
        }

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MainWindow");

        #endregion

        #region 实例变量

        private SessionTreeVM sessionTreeVM;
        private Dictionary<SessionListViewEnum, SessionListView> viewList;
        private SessionListViewEnum currentViewType;
        private SessionListView currentView;
        private ServiceAgent serviceAgent;

        #endregion

        #region 属性

        /// <summary>
        /// 当前选中的会话
        /// </summary>
        public XTermSession SelectedSession { get; private set; }

        /// <summary>
        /// 当前选中的会话ViewModel
        /// </summary>
        public XTermSessionVM SelectedSessionVM { get; private set; }

        #endregion

        #region 实例变量

        #endregion

        #region 构造方法

        public SessionListWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;
            this.sessionTreeVM = MTermApp.Context.CreateSessionTreeVM();
            this.viewList = new Dictionary<SessionListViewEnum, SessionListView>();
            this.SwitchView(SessionListViewEnum.DataGrid);
        }

        private void OpenSession(XTermSessionVM sessionVM)
        {
            this.SelectedSessionVM = sessionVM;
            this.SelectedSession = sessionVM.Session;

            base.DialogResult = true;
        }

        private SessionListView CreateListView(SessionListViewEnum listViewEnum)
        {
            switch (listViewEnum)
            {
                //case SessionListViewEnum.GroupDataList:
                //case SessionListViewEnum.TreeList: return new TreeListUserControl();
                case SessionListViewEnum.DataList:
                case SessionListViewEnum.DataGrid: return new DataGridUserControl();
                default:
                    throw new NotImplementedException();
            }
        }

        private void SwitchView(SessionListViewEnum listViewEnum)
        {
            SessionListView listView;
            if (!this.viewList.TryGetValue(listViewEnum, out listView))
            {
                listView = this.CreateListView(listViewEnum);
                listView.OpenSessionEvent += ListView_OpenSessionEvent;
                this.viewList[listViewEnum] = listView;
            }

            ContentControlSessionList.Content = listView;
            listView.SessionTreeVM = this.sessionTreeVM;
            if (this.currentView != null)
            {
                this.currentView.OnUnload();
                listView.CurrentGroup = this.currentView.CurrentGroup;
            }

            if (listView is DataGridUserControl) 
            {
                DataGridUserControl dataGridUserControl = listView as DataGridUserControl;
                dataGridUserControl.SetMode(listViewEnum);
            }

            listView.OnLoad();

            this.currentViewType = listViewEnum;
            this.currentView = listView;
        }

        #endregion

        #region 事件处理器

        private void ListView_OpenSessionEvent(XTermSessionVM sessionVM)
        {
            this.OpenSession(sessionVM);
        }

        private void ButtonCreateSession_Click(object sender, RoutedEventArgs e)
        {
            CreateSessionOptionTreeWindow window = new CreateSessionOptionTreeWindow();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (!(bool)window.ShowDialog())
            {
                return;
            }

            XTermSession session = window.Session;

            // 在数据库里新建会话
            int code = MTermApp.Context.ServiceAgent.AddSession(session);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("新建会话失败, {0}, {1}", code, ResponseCode.GetMessage(code));
                return;
            }

            this.SelectedSession = session;

            base.DialogResult = true;
        }

        private void ButtonDeleteSession_Click(object sender, RoutedEventArgs e)
        {
            SessionTreeNodeVM selectedItem = this.sessionTreeVM.Context.SelectedItem as SessionTreeNodeVM;

            if (selectedItem == null)
            {
                MTMessageBox.Info("请选择要删除的会话或者分组");
                return;
            }

            switch (selectedItem.NodeType)
            {
                case SessionTreeNodeTypeEnum.Session:
                    {
                        XTermSessionVM sessionVM = selectedItem as XTermSessionVM;

                        if (!MTMessageBox.Confirm("确定要删除{0}吗?", sessionVM.Name))
                        {
                            return;
                        }

                        int code = this.serviceAgent.DeleteSession(sessionVM.ID.ToString());
                        if (code != ResponseCode.SUCCESS)
                        {
                            MTMessageBox.Info("删除会话失败, {0}", code);
                            return;
                        }

                        if (sessionVM.Parent != null)
                        {
                            sessionVM.Remove();
                        }
                        else
                        {
                            // 是根节点
                            this.sessionTreeVM.Roots.Remove(sessionVM);
                        }

                        break;
                    }

                case SessionTreeNodeTypeEnum.Group:
                    {
                        this.sessionTreeVM.DeleteSelectedGroup();
                        break;
                    }

                case SessionTreeNodeTypeEnum.GobackGroup:
                    {
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        private void ButtonOpenSession_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonSwitchView_Click(object sender, RoutedEventArgs e)
        {
            SessionListViewEnum targetView = this.currentViewType + 1;
            if (!Enum.IsDefined<SessionListViewEnum>(targetView))
            {
                targetView = SessionListViewEnum.DataGrid;
            }

            this.SwitchView(targetView);
        }

        private void ButtonCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            SessionGroupVM currentGroup = this.currentView.CurrentGroup;
            string groupId = currentGroup == null ? string.Empty : currentGroup.ID.ToString();

            CreateSessionGroupWindow createSessionGroupWindow = new CreateSessionGroupWindow(groupId);
            createSessionGroupWindow.OnSessionGroupCreated += CreateSessionGroupWindow_OnSessionGroupCreated;
            createSessionGroupWindow.OnSessionGroupDeleted += CreateSessionGroupWindow_OnSessionGroupDeleted;
            createSessionGroupWindow.Owner = this;
            if ((bool)createSessionGroupWindow.ShowDialog())
            {
            }
        }

        private void CreateSessionGroupWindow_OnSessionGroupDeleted(SessionGroup sessionGroup)
        {
            TreeNodeViewModel treeNodeViewModel;
            if (this.sessionTreeVM.TryGetNode(sessionGroup.ID.ToString(), out treeNodeViewModel))
            {
                treeNodeViewModel.Remove();
            }
        }

        private void CreateSessionGroupWindow_OnSessionGroupCreated(SessionGroup sessionGroup)
        {
            SessionGroupVM sessionGroupVM = new SessionGroupVM(this.sessionTreeVM.Context, sessionGroup);

            if (string.IsNullOrEmpty(sessionGroup.ParentId))
            {
                // 说明是根节点
                this.sessionTreeVM.AddRootNode(sessionGroupVM);
            }
            else
            {
                TreeNodeViewModel parentNode;
                if (this.sessionTreeVM.TryGetNode(sessionGroup.ParentId, out parentNode))
                {
                    parentNode.Add(sessionGroupVM);
                }
            }
        }

        #endregion
    }
}
