using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Panel
{
    public interface IAddonOverlayPanel : IAddonPanel
    {
        /// <summary>
        /// OverlayPanel所属的Tab页面
        /// </summary>
        IClientShellTab OwnerTab { get; set; }
    }
}
