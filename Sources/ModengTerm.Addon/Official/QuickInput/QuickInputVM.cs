using ModengTerm.Base.Addon.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Official.QuickInput
{
    public class QuickInputVM : SessionPanel
    {
        #region 属性

        public BindableCollection<CommandVM> CommandList { get; private set; }

        #endregion

        #region 公开接口

        public void ReloadCommandList()
        {
            CommandList.Clear();
            CommandList.AddRange(ServiceAgent.GetShellCommands(Session.ID).Select(v => new CommandVM(v)));
        }

        #endregion

        #region SessionPanel

        public override void OnInitialize()
        {
            base.OnInitialize();

            CommandList = new BindableCollection<CommandVM>();
            CommandList.AddRange(ServiceAgent.GetShellCommands(Session.ID).Select(v => new CommandVM(v)));
        }

        public override void OnRelease()
        {
            CommandList.Clear();

            base.OnRelease();
        }

        public override void OnReady()
        {
        }

        #endregion
    }
}
