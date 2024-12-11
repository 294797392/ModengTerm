using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    public interface IDrawingWallpaper : IDrawingObject
    {
        /// <summary>
        /// 壁纸大小和位置
        /// </summary>
        VTRect Rect { get; set; }

        /// <summary>
        /// 背景类型
        /// </summary>
        WallpaperTypeEnum PaperType { get; set; }

        /// <summary>
        /// 背景特效
        /// </summary>
        EffectTypeEnum EffectType { get; set; }

        /// <summary>
        /// 背景颜色
        /// 如果背景是动态背景，那么该颜色是动态背景的主色调
        /// </summary>
        string BackgroundColor { get; set; }

        /// <summary>
        /// 如果是图片或者动态背景，那么该值表示动态背景的Uri
        /// </summary>
        string Uri { get; set; }
    }
}
