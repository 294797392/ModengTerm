using ModengTerm.Addon.Interactive;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Session;
using System;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerVM : SidePanel
    {
        public SessionTreeVM ResourceManagerTree { get; private set; }

        protected override void OnInitialize()
        {
            this.ResourceManagerTree = VMUtils.CreateSessionTreeVM(false, true);
            this.ResourceManagerTree.Roots[0].Name = "会话列表";
            this.ResourceManagerTree.ExpandAll();

            Console.WriteLine(string.Format("{0}, OnInitialize", this.Name));
        }

        protected override void OnRelease()
        {
            //Console.WriteLine(string.Format("{0}, OnRelease", this.Name));
        }

        protected override void OnLoaded()
        {
            //Console.WriteLine(string.Format("{0}, OnLoaded", this.Name));
        }

        protected override void OnUnload()
        {
            //Console.WriteLine(string.Format("{0}, OnUnload", this.Name));
        }
    }
}

