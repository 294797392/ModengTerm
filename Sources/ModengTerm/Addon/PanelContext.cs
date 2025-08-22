using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public abstract class PanelContext
    {
        public PanelMetadata Definition { get; private set; }

        /// <summary>
        /// SidePanel所属的插件
        /// </summary>
        public AddonModule OwnerAddon { get; private set; }

        public PanelContext(AddonModule ownerAddon, PanelMetadata definition)
        {
            this.OwnerAddon = ownerAddon;
            this.Definition = definition;
        }
    }

    public class OverlayPanelContext : PanelContext
    {
        public OverlayPanelContext(AddonModule ownerAddon, PanelMetadata definition) : base(ownerAddon, definition)
        {
        }
    }
}
