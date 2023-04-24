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
        /// 可创建的会话类型列表
        /// </summary>
        public BindableCollection<SessionTypeVM> SessionTypes { get; private set; }

        /// <summary>
        /// 用户已经创建了的会话列表
        /// </summary>
        public BindableCollection<SessionVM> SessionList { get; private set; }

        public MainWindowVM()
        {
            this.SessionTypes = new BindableCollection<SessionTypeVM>();
            this.SessionList = new BindableCollection<SessionVM>();

            #region 加载会话类型列表

            List<SessionDefinition> sessions = XTermApp.Context.ServiceAgent.GetSessionDefinitions();
            foreach (SessionDefinition sessionDefinition in sessions)
            {
                SessionTypeVM sessionTypeVM = new SessionTypeVM()
                {
                    ID = sessionDefinition.ID,
                    Name = sessionDefinition.Name,
                    Description = sessionDefinition.Description,
                    IconURI = sessionDefinition.Icon,
                    ProviderEntry = sessionDefinition.ProviderEntry,
                    Type = (SessionTypeEnum)sessionDefinition.Type
                };
                this.SessionTypes.Add(sessionTypeVM);
            }

            #endregion

            #region 加载已存在的会话列表

            List<SessionDM> sessionList = XTermApp.Context.ServiceAgent.GetSessions();
            foreach (SessionDM session in sessionList)
            {
                SessionVM sessionVM = new SessionVM() 
                {
                };
            }

            #endregion
        }
    }
}
