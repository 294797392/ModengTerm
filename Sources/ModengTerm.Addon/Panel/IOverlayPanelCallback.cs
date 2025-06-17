using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Panel
{
    public interface IOverlayPanelCallback : IPanelCallback
    {
        /// <summary>
        /// OverlayPanel所属的Tab页面
        /// </summary>
        IShellTab OwnerTab { get; set; }
    }
}
