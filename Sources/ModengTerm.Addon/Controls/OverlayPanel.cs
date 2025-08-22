using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModengTerm.Addon.Controls
{
    public abstract class OverlayPanel : Panel
    {
        /// <summary>
        /// OverlayPanel所属的Tab页面
        /// </summary>
        public IClientShellTab OwnerTab { get; set; }
    }
}
