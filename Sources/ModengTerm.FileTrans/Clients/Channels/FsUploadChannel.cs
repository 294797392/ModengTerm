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
        /// 上传指定内容到指定位置
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        public abstract void UploadSlice(byte[] bytes, int offset, int len);
    }
}
