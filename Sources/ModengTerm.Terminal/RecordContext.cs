using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 存储录像状态
    /// </summary>
    public class RecordContext
    {
        /// <summary>
        /// 是否正在录像
        /// </summary>
        public RecordStateEnum State { get; set; }

        /// <summary>
        /// 维护录像文件
        /// 封装对录像文件的读写操作
        /// </summary>
        public PlaybackFile PlaybackFile { get; private set; }

        public RecordContext()
        {
            this.PlaybackFile = new PlaybackFile();
        }
    }
}
