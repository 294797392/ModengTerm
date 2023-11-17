using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ServiceAgents.DataModels
{
    /// <summary>
    /// 存储回放文件里的一帧信息
    /// </summary>
    public class PlaybackFrame
    {
        /// <summary>
        /// 该帧的时间戳
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 该帧数据
        /// 也就是从SSH主机收到的数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
