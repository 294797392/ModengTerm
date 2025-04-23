using ModengTerm.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.ClientStubs
{
    /// <summary>
    /// 访问服务端
    /// </summary>
    public abstract class ClientStub
    {
        /// <summary>
        /// Stub事件
        /// </summary>
        public event Action<ClientStub, StubEventTypes, string> OnEvent;

        protected void RaiseEvent(StubEventTypes evt, string evp)
        {
            this.OnEvent?.Invoke(this, evt, evp);
        }
    }
}