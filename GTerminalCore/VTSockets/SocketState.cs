using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GardeniaTerminalCore
{
    public enum SocketState
    {
        /// <summary>
        /// 数据流处于初始化状态
        /// </summary>
        Init,

        /// <summary>
        /// 所有准备工作已就绪，可以开始解析终端数据流了
        /// </summary>
        Ready
    }
}