using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.Find
{
    public class FindAddon
    {
        public void MenuItemOpenFindWindow_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            FindWindowMgr.Show(shellSessionVM);
            //shellSessionVM.Panel.ChangeVisible("0C1F6D60-F6ED-4D01-B5B6-1812EA55286A");
        }
    }
}
