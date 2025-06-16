using DotNEToolkit;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 提供创建插件可以使用的对象的功能
    /// </summary>
    public abstract class HostFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("HostFactory");

        public abstract List<IHostSidePanel> CreateSidePanels(List<PanelDefinition> definitions);

        public abstract List<IHostOverlayPanel> CreateOverlayPanels(List<PanelDefinition> definitions);
    }
}