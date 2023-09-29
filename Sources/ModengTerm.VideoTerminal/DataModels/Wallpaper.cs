using ModengTerm.Terminal.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
