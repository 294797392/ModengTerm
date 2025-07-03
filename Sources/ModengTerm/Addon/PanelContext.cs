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
    public abstract class PanelContext
    {
        public PanelDefinition Definition { get; private set; }

        /// <summary>
        /// SidePanel所属的插件
        /// </summary>
        public AddonModule OwnerAddon { get; private set; }

        public PanelContext(AddonModule ownerAddon, PanelDefinition definition)
        {
            this.OwnerAddon = ownerAddon;
            this.Definition = definition;
        }
    }

    public class SidePanelContext : PanelContext
    {
        public SidePanel Panel { get; set; }

        /// <summary>
        /// 是否添加到了窗口里
        /// </summary>
        public bool IsAttached { get; set; }

        public SidePanelContext(AddonModule ownerAddon, PanelDefinition definition) :
            base(ownerAddon, definition)
        {
        }
    }

    public class OverlayPanelContext : PanelContext
    {
        public OverlayPanelContext(AddonModule ownerAddon, PanelDefinition definition) : base(ownerAddon, definition)
        {
        }
    }
}
