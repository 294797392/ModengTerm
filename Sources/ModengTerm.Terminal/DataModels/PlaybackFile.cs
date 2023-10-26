using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.DataModels
{
    /// <summary>
    /// 存储一个可以回放的文件
    /// </summary>
    public class PlaybackFile : ModelBase
    {
        /// <summary>
        /// 该文件里的所有帧
        /// </summary>
        public List<PlaybackFrame> Frames { get; set; }

        public PlaybackFile()
        {
            this.Frames = new List<PlaybackFrame>();
        }
    }
}
