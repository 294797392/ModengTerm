using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Session;
using System.Windows.Controls;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    /// <summary>
    /// ResourceManagerUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SessionExplorerPanel : UserControl, IAddonSidePanel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SessionExplorerUserControl");

        #endregion

        #region 实例变量

        private SessionTreeVM resourceManagerTreeVM;

        #endregion

        #region 构造方法

        public SessionExplorerPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region 事件处理器

        private void SessionTreeViewUserControl_TreeViewMouseDoubleClick(UserControls.SessionTreeViewUserControl arg1, SessionTreeNodeVM selectedItem)
        {
            if (selectedItem == null)
            {
                return;
            }

            SessionTreeNodeVM sessionNode = selectedItem as SessionTreeNodeVM;
            if (sessionNode == null)
            {
                return;
            }

            if (sessionNode.NodeType != SessionTreeNodeTypeEnum.Session)
            {
                return;
            }

            XTermSessionVM sessionVM = sessionNode as XTermSessionVM;

            // 打开会话
            ClientFactory factory = ClientFactory.GetFactory();
            IClientWindow hostWindow = factory.GetHostWindow();
            hostWindow.OpenSession(sessionVM.Session);
        }

        #endregion

        #region ISidePanel

        public void OnInitialize()
        {
            this.resourceManagerTreeVM = VMUtils.CreateSessionTreeVM(false, true);
            this.resourceManagerTreeVM.Roots[0].Name = "会话列表";
            this.resourceManagerTreeVM.ExpandAll();

            SessionTreeViewUserControl.DataContext = this.resourceManagerTreeVM;

            logger.InfoFormat(string.Format("{0}, OnInitialize", this.Name));
        }

        public void OnRelease()
        {
            logger.InfoFormat(string.Format("{0}, OnRelease", this.Name));
        }

        public void OnLoaded()
        {
            logger.InfoFormat(string.Format("{0}, OnLoaded", this.Name));
        }

        public void OnUnload()
        {
            logger.InfoFormat(string.Format("{0}, OnUnload", this.Name));
        }

        #endregion
    }
}
