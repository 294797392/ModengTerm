using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WPFToolkit.MVVM;
using ModengTerm.Controls;
using log4net.Repository.Hierarchy;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.Terminal;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 打开的所有会话列表ViewModel
    /// </summary>
    public class OpenedSessionsVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("OpenedSessionsVM");

        /// <summary>
        /// 最后一个打开会话的按钮
        /// </summary>
        public static readonly OpenSessionVM OpenSessionVM = new OpenSessionVM();

        #endregion

        #region 公开事件

        /// <summary>
        /// 当会话被完全打开之后触发
        /// </summary>
        public event Action<OpenedSessionsVM, OpenedSessionVM> OnSessionOpened;

        #endregion

        #region 公开属性

        /// <summary>
        /// 打开的所有会话列表
        /// </summary>
        public BindableCollection<SessionItemVM> SessionList { get; private set; }

        /// <summary>
        /// 打开的所有类型是Shell的会话列表
        /// </summary>
        public IEnumerable<ShellSessionVM> ShellSessions { get { return this.SessionList.OfType<ShellSessionVM>(); } }

        /// <summary>
        /// 选中的会话
        /// </summary>
        public SessionItemVM SelectedSession
        {
            get { return this.SessionList.SelectedItem; }
            set
            {
                this.SessionList.SelectedItem = value;
                this.NotifyPropertyChanged("SelectedSession");
            }
        }

        #endregion

        #region 构造方法

        public OpenedSessionsVM()
        {
            this.SessionList = new BindableCollection<SessionItemVM>();
            this.SessionList.Add(OpenSessionVM);
        }

        #endregion

        #region 公开接口

        public ISessionContent OpenSession(XTermSession session)
        {
            ISessionContent content = SessionContentFactory.Create(session);
            content.Session = session;
            
            OpenedSessionVM viewModel = OpenedSessionVMFactory.Create(session);
            viewModel.ID = session.ID;
            viewModel.Name = session.Name;
            viewModel.Description = session.Description;
            viewModel.Content = content as DependencyObject;
            viewModel.ServiceAgent = MTermApp.Context.ServiceAgent;

            // 先加到打开列表里，这样在打开列表里就不会重复添加会话的上下文菜单
            int index = this.SessionList.IndexOf(OpenSessionVM);
            this.SessionList.Insert(index, viewModel);
            this.SelectedSession = viewModel;

            // 在OnLoad事件里执行打开会话的动作
            // 因为初始化终端需要知道当前的界面大小，从而计算行大小和列大小
            FrameworkElement frameworkElement = content as FrameworkElement;
            frameworkElement.DataContext = viewModel;
            frameworkElement.Loaded += SessionContent_Loaded;  // Content完全显示出来会触发这个事件

            return content;
        }

        public void CloseSession(OpenedSessionVM session)
        {
            ISessionContent content = session.Content as ISessionContent;
            if (MTermUtils.IsTerminal((SessionTypeEnum)content.Session.Type))
            {
                UserControl userControl = content as UserControl;
            }
            content.Close();

            this.SessionList.Remove(session);
        }

        #endregion

        #region 事件处理器

        private void SessionContent_Loaded(object sender, RoutedEventArgs e)
        {
            // 此时所有的界面都加载完了，可以真正打开Session了
            ISessionContent content = sender as ISessionContent;

            // 要反注册事件，不然每次显示界面就会多打开一个VideoTerminal
            FrameworkElement frameworkElement = content as FrameworkElement;
            frameworkElement.Loaded -= this.SessionContent_Loaded;

            int code = content.Open();
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开会话失败, {0}", code);
            }

            this.OnSessionOpened?.Invoke(this, frameworkElement.DataContext as OpenedSessionVM);
        }

        #endregion
    }
}
