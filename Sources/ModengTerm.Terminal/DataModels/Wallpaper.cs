using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.Enumerations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ModengTerm.Terminal.DataModels
{
    /// <summary>
    /// 表示一个壁纸
    /// </summary>
    public class Wallpaper
    {
        /// <summary>
        /// 壁纸类型
        /// </summary>
        [EnumDataType(typeof(WallpaperTypeEnum))]
        public int Type { get; set; }

        /// <summary>
        /// 壁纸的路径
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// 特效
        /// </summary>
        [JsonProperty("effect")]
        public int Effect { get; set; }

        /// <summary>
        /// 主色调
        /// 如果背景是图片的话，这个字段定义该图片的主色调
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }
    }
}
