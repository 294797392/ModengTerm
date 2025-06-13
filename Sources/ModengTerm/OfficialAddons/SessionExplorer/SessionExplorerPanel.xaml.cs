using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.ViewModel.Session;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    /// <summary>
    /// ResourceManagerUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SessionExplorerPanel : UserControl
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("SessionExplorerUserControl");

        #endregion

        #region 实例变量

        //private SessionTreeVM resourceManagerTreeVM;

        #endregion

        #region 构造方法

        public SessionExplorerPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region IContentHook

        private void CreateDefaultValue() 
        {
        }

        public void OnInitialize()
        {
            throw new RefactorImplementedException();
            //this.resourceManagerTreeVM = VTApp.Context.ResourceManagerTreeVM;
            //SessionTreeViewUserControl.ViewModel = this.resourceManagerTreeVM;
        }

        public void OnRelease()
        {

        }

        public void OnLoaded()
        {
        }

        public void OnUnload()
        {
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
            HostFactory factory = HostFactory.GetFactory();
            IHostWindow window = factory.GetHostWindow();
            window.OpenSession(sessionVM.Session);
        }

        #endregion
    }
}
