using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Rendering.Background
{
    public abstract class DrawingWallpaper : DrawingObject
    {
        protected VTWallpaper wallpaper;

        /// <summary>
        /// 获取该壁纸大小
        /// </summary>
        protected Rect Size
        {
            get
            {
                VTWallpaper wallpaper = this.documentElement as VTWallpaper;

                // -5,-5,+5,+5是为了消除边距
                return new Rect(-5, -5, wallpaper.Rect.Width + 5, wallpaper.Rect.Height + 5);
            }
        }

        protected override void OnInitialize()
        {
            this.wallpaper = this.documentElement as VTWallpaper;
        }
    }
}
