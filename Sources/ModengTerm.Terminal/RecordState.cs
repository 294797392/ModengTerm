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
    public class RecordState
    {
        /// <summary>
        /// 是否正在录像
        /// </summary>
        public bool IsRecord { get; set; }

        /// <summary>
        /// 录像文件的名字
        /// </summary>
        public string PlaybackFileName { get; set; }
    }
}
