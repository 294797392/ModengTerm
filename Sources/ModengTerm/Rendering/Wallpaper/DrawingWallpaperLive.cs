using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModengTerm.Rendering.Background
{
    /// <summary>
    /// 使用VideoDrawing播放视频
    /// https://learn.microsoft.com/zh-cn/dotnet/desktop/wpf/graphics-multimedia/how-to-play-media-using-a-videodrawing?view=netframeworkdesktop-4.8
    /// </summary>
    public class DrawingWallpaperLive : DrawingWallpaper
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
        }

        protected override void OnDraw(DrawingContext dc)
        {
        }
    }
}


