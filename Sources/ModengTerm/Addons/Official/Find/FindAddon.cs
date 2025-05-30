using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.Find
{
    public class FindAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand(AddonCommands.CMD_SELECTED_SESSION_CHANGED, this.SelectedSessionChanged);
            this.RegisterCommand("E71680AF-F5D8-4F18-A0BF-BB60DD4DAA1C", OpenFindWindowCommandHandler);
        }

        protected override void OnDeactive()
        {
        }

        private void SelectedSessionChanged(CommandArgs e)
        {
            // 如果选中的会话是Shell会话并且显示了查找窗口，那么搜索选中的会话

            // TODO：打开搜索窗口的同时，新打开了一个会话，此时会话里的VideoTerminal为空，因为还没打开完
            ShellSessionVM selectedSession = e.OpenedSession as ShellSessionVM;
            if (selectedSession != null && selectedSession.VideoTerminal != null)
            {
                if (FindWindowMgr.WindowShown)
                {
                    FindWindowMgr.Show(selectedSession);
                }
            }
        }

        private void OpenFindWindowCommandHandler(CommandArgs context)
        {
            ShellSessionVM shellSessionVM = context.OpenedSession as ShellSessionVM;
            if (shellSessionVM == null)
            {
                return;
            }

            FindWindowMgr.Show(shellSessionVM);
        }
    }
}
