using ModengTerm.Addon.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModel.Panel
{
    public class HostOverlayPanel : HostPanel, IHostOverlayPanel
    {
        public override bool IsOpened => throw new NotImplementedException();

        public override void Open()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }
    }
}
