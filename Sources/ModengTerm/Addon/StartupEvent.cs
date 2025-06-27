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
        public ClientPanel Panel { get; set; }

        public PanelDefinition Definition { get; private set; }

        public PanelState(PanelDefinition definition) 
        {
            this.Definition = definition;
        }
    }

    /// <summary>
    /// 存储当一个事件被激活之后，该事件所要激活的信息
    /// </summary>
    public class StartupEvent
    {
        public List<PanelState> SidePanels { get; private set; }

        public List<PanelState> OverlayPanels { get; private set; }

        public StartupEvent()
        {
            this.SidePanels = new List<PanelState>();
            this.OverlayPanels = new List<PanelState>();
        }
    }
}
