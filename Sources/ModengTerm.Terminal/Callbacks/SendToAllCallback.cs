using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Callbacks
{
    /// <summary>
    /// 发送到所有窗口的委托
    /// </summary>
    /// <param name="text"></param>
    public delegate void SendToAllTerminalCallback(string text);
}
