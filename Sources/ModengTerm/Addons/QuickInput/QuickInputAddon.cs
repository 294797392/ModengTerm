using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.QuickInput
{
    public class QuickInputAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("QuickInputAddon.ShowQuickInputView", this.ExecuteShowQuickInputViewCommand);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {
        }

        private void ExecuteShowQuickInputViewCommand()
        {
            ShellSessionVM shellSessionVM = this.MainWindow.SelectedSession as ShellSessionVM;
            shellSessionVM.Panel.ChangeVisible("0C1F6D60-F6ED-4D01-B5B6-1812EA55286A");
        }
    }
}
