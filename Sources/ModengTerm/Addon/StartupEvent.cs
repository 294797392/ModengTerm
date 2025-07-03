using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
using ModengTerm.ViewModel.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public class PanelState
    {
        public SidePanel Panel { get; set; }

        public PanelDefinition Definition { get; private set; }

        /// <summary>
        /// 是否添加到了窗口里
        /// </summary>
        public bool AddToWindow { get; set; }

        /// <summary>
        /// SidePanel所属的插件
        /// </summary>
        public AddonModule OwnerAddon { get; set; }

        public PanelState(AddonModule ownerAddon, PanelDefinition definition) 
        {
            this.OwnerAddon = ownerAddon;
            this.Definition = definition;
        }
    }
}
