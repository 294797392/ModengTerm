using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base;
using XTerminal.Base.Definitions;

namespace VideoTerminal.ViewModels
{
    public class CreateSessionVM : ViewModelBase
    {
        /// <summary>
        /// 所有可创建的会话列表
        /// </summary>
        public BindableCollection<SessionDefinition> SessionList { get; private set; }

        public CreateSessionVM()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            this.SessionList = new BindableCollection<SessionDefinition>();
            this.SessionList.AddRange(XTermApp.Context.ServiceAgent.GetSessionDefinitions());
        }
    }
}
