using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Session.Definitions;
using XTerminal.Sessions;

namespace XTerminal.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        /// <summary>
        /// 用户已经创建了的会话列表
        /// </summary>
        public BindableCollection<SessionVM> SessionList { get; private set; }

        public MainWindowVM()
        {
            this.SessionList = new BindableCollection<SessionVM>();

            #region 加载已存在的会话列表

            List<SessionDM> sessionList = XTermApp.Context.ServiceAgent.GetSessions();
            foreach (SessionDM session in sessionList)
            {
                SessionVM sessionVM = new SessionVM()
                {
                    ID = session.ID,
                    Name = session.Name,
                    Description = session.Description,
                };

                this.SessionList.Add(sessionVM);
            }

            #endregion
        }
    }
}
