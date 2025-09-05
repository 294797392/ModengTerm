using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Enumerations
{
    public enum FsTransferTypeEnum
    {
        /// <summary>
        /// 客户端传输到客户端
        /// </summary>
        ClientToClient,

        /// <summary>
        /// 客户端传输到服务器
        /// </summary>
        ClientToServer,

        /// <summary>
        /// 服务器传输到服务器
        /// </summary>
        ServerToServer,

        /// <summary>
        /// 服务器传输到客户端
        /// </summary>
        ServerToClient
    }
}
