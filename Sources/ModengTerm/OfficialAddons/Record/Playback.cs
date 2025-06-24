using ModengTerm.Addons;
using ModengTerm.Base.DataModels;
using Newtonsoft.Json;

namespace ModengTerm.OfficialAddons.Record
{
    /// <summary>
    /// 存储回放信息
    /// 一个Playback对应一个录像文件
    /// 录像文件由多个帧组成
    /// 每一帧分帧头和帧体，帧头是20字节，帧体大小不固定
    /// 在录像的时候，要把数据实时写入文件，而不是缓存起来最后一次性写入文件，因为数据量有可能会很大
    /// </summary>
    public class Playback : StorageObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 该回放文件所属的会话信息
        /// 保存回放文件的时候会拷贝一份会话信息
        /// </summary>
        [JsonProperty("session")]
        public XTermSession Session { get; set; }

        /// <summary>
        /// 录像文件完整路径
        /// </summary>
        [JsonProperty("fullPath")]
        public string FullPath { get; set; }
    }
}
