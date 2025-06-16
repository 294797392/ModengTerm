using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Client
{
    public interface IPanelBase
    {
        string Name { get; set; }

        void OnInitialize(HostContext context);
        void OnRelease();
        void OnLoaded();
        void OnUnload();
    }
}
