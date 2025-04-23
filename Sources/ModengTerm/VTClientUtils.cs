using ModengTerm.Base;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.DataModels;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModengTerm
{
    public static class VTClientUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTClientUtils");


        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="parameters">要传递到PanelContentVM里的参数</param>
        /// <param name="matchType"></param>
        /// <returns></returns>
        public static PanelVM PanelDefinition2PanelVM(PanelDefinition panel)
        {
            PanelVM panelVM = new PanelVM();
            panelVM.ID = panel.ID;
            panelVM.Name = panel.Name;

            foreach (PanelItemDefinition panelItem in panel.Items)
            {
                PanelItemVM panelItemVM = new PanelItemVM(panelItem);
                panelItemVM.ID = panelItem.ID;
                panelItemVM.Name = panelItem.Name;
                panelItemVM.IconURI = panelItem.Icon;
                panelItemVM.ClassName = panelItem.ClassName;
                panelItemVM.VMClassName = panelItem.VMClassName;
                panelVM.AddMenuItem(panelItemVM);
            }

            return panelVM;
        }
    }
}
