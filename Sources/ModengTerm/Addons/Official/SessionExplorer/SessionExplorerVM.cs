using DotNEToolkit.Media.Video;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.SessionExplorer
{
    public class SessionExplorerVM : PanelContentVM
    {
        public SessionTreeVM ResourceManagerTree { get; private set; }

        public override void OnInitialize()
        {
            base.OnInitialize();

            ResourceManagerTree = MTermApp.Context.ResourceManagerTreeVM;
            //Console.WriteLine(string.Format("{0}, OnInitialize", this.Name));
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

