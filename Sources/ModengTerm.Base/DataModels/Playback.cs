using DotNEToolkit.DataModels;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    /// <summary>
    /// 存储回放信息
    /// 一个Playback对应一个录像文件
    /// 录像文件由多个帧组成
    /// 每一帧分帧头和帧体，帧头是20字节，帧体大小不固定
    /// 在录像的时候，要把数据实时写入文件，而不是缓存起来最后一次性写入文件，因为数据量有可能会很大
    /// </summary>
    public class Playback : ModelBase
    {
        #region 属性

        /// <summary>
        /// 该回放文件所属的会话信息
        /// 保存回放文件的时候会拷贝一份会话信息
        /// </summary>
        public XTermSession Session { get; set; }

        #endregion

        #region 构造方法

        public Playback()
        {
        }

        #endregion
    }
}
