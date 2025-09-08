using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients.Channels
{
    public abstract class FsUploadChannel : FsChannel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        public abstract void UploadSlice(byte[] bytes);
    }
}
