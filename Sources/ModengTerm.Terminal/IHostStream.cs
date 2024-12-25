using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public interface IHostStream
    {
        int Send(byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>
        /// 如果连接断开返回-1
        /// 超时返回0
        /// </returns>
        int Receive(byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="timeout"></param>
        /// <returns>
        /// 如果连接断开返回-1
        /// 超时返回0
        /// </returns>
        int Receive(byte[] buffer, int timeout);

        int Receive(byte[] buffer, int offset, int count);

        int Receive(byte[] buffer, int offset, int count, int timeout);
    }
}
