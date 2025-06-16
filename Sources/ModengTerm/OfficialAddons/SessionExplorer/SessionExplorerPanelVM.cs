using ModengTerm.Addon.Extensions;
using ModengTerm.Addon.Interactive;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Session;
using System;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerPanelVM : SidePanel
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("");

        public SessionTreeVM ResourceManagerTree { get; private set; }

        public override void OnInitialize()
        {
            this.ResourceManagerTree = VMUtils.CreateSessionTreeVM(false, true);
            this.ResourceManagerTree.Roots[0].Name = "会话列表";
            this.ResourceManagerTree.ExpandAll();

            logger.InfoFormat(string.Format("{0}, OnInitialize", this.Name));
        }

        public override void OnRelease()
        {
            logger.InfoFormat(string.Format("{0}, OnRelease", this.Name));
        }

        public override void OnLoaded()
        {
            logger.InfoFormat(string.Format("{0}, OnLoaded", this.Name));
        }

        public override void OnUnload()
        {
            logger.InfoFormat(string.Format("{0}, OnUnload", this.Name));
        }
    }
}

