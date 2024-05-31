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
    public class OpenedSessionListVM : ViewModelBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("OpenedSessionListVM");

        public static readonly OpenSessionVM OpenSessionVM = new OpenSessionVM();

        public BindableCollection<SessionItemVM> SessionList { get; private set; }

        public IEnumerable<ShellSessionVM> ShellSessions { get { return this.SessionList.OfType<ShellSessionVM>(); } }

        public SessionItemVM SelectedSession
        {
            get { return this.SessionList.SelectedItem; }
            set
            {
                this.SessionList.SelectedItem = value;
                this.NotifyPropertyChanged("SelectedSession");
            }
        }

        public OpenedSessionListVM()
        {
            this.SessionList = new BindableCollection<SessionItemVM>();
            this.SessionList.Add(OpenSessionVM);
        }

        public ISessionContent OpenSession(XTermSession session)
        {
            // 先初始化UI，等UI显示出来在打开Session
            // 因为初始化终端需要知道当前的界面大小，从而计算行大小和列大小

            ISessionContent content = SessionContentFactory.Create(session);
            OpenedSessionVM viewModel = OpenedSessionVMFactory.Create(session);

            // 给ViewModel赋值
            viewModel.ID = Guid.NewGuid().ToString();
            viewModel.Name = session.Name;
            viewModel.Description = session.Description;
            viewModel.Content = content as DependencyObject;
            viewModel.ServiceAgent = MTermApp.Context.ServiceAgent;

            // 给SessionContent赋值
            content.Session = session;

            UserControl userControl = content as UserControl;
            userControl.DataContext = viewModel;
            userControl.Loaded += SessionContent_Loaded;  // Content完全显示出来会触发这个事件

            // 加到打开按钮前面
            int index = this.SessionList.IndexOf(OpenSessionVM);
            this.SessionList.Insert(index, viewModel);
            this.SelectedSession = viewModel;

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

        #region 事件处理器

        private void SessionContent_Loaded(object sender, RoutedEventArgs e)
        {
            // 此时所有的界面都加载完了，可以真正打开Session了
            ISessionContent content = sender as ISessionContent;

            // 要反注册事件，不然每次显示界面就会多打开一个VideoTerminal
            UserControl userControl = content as UserControl;
            userControl.Loaded -= this.SessionContent_Loaded;

            int code = content.Open();
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开会话失败, {0}", code);
            }

            OpenedSessionVM sessionVM = userControl.DataContext as OpenedSessionVM;
        }

        #endregion
    }
}
