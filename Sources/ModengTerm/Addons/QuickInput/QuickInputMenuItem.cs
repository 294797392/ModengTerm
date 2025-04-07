using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModengTerm.Addons.QuickInput
{
    public class QuickInputMenuItem
    {
        public void SwitchQuickInputViewHandler(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            shellSessionVM.Panel.ChangeVisible("0C1F6D60-F6ED-4D01-B5B6-1812EA55286A");
        }
    }
}
