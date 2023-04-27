using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal;
using XTerminal.Base;
using XTerminal.Session.Definitions;
using XTerminal.Sessions;
using XTerminal.ViewModels;

namespace XTerminal.ViewModels
{
    public class CreateSessionVM : ViewModelBase
    {
        /// <summary>
        /// 所有可创建的会话列表
        /// </summary>
        public BindableCollection<SessionTypeVM> SessionList { get; private set; }

        public CreateSessionVM()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            this.SessionList = new BindableCollection<SessionTypeVM>();

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
                this.SessionList.Add(sessionTypeVM);
            }

            #endregion

        }
    }
}
