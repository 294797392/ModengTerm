using DotNEToolkit.Media.Video;
using ModengTerm.Addon.ViewModel;
using ModengTerm.Base;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerVM : WindowPanel
    {
        public SessionTreeVM ResourceManagerTree { get; private set; }

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.ResourceManagerTree = VMUtils.CreateSessionTreeVM(false, true);
            this.ResourceManagerTree.Roots[0].Name = "会话列表";
            this.ResourceManagerTree.ExpandAll();

            Console.WriteLine(string.Format("{0}, OnInitialize", this.Name));
        }

        public override void OnRelease()
        {
            //Console.WriteLine(string.Format("{0}, OnRelease", this.Name));

            base.OnRelease();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            //Console.WriteLine(string.Format("{0}, OnLoaded", this.Name));
        }

        public override void OnUnload()
        {
            //Console.WriteLine(string.Format("{0}, OnUnload", this.Name));

            base.OnUnload();
        }
    }
}

