using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class QuickCommandManager : PanelContentVM
    {
        /// <summary>
        /// 该会话的所有快捷命令
        /// </summary>
        public BindableCollection<QuickCommandVM> ShellCommands { get; private set; }

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.ShellCommands = new BindableCollection<QuickCommandVM>();
            this.ShellCommands.AddRange(this.ServiceAgent.GetShellCommands(this.Session.ID).Select(v => new QuickCommandVM(v)));
        }
    }
}
