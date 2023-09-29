using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Rendering.Wallpaper
{
    /// <summary>
    /// 表示一个需要实时刷新的动态效果
    /// </summary>
    public abstract class EffectRenderer
    {
        /// <summary>
        /// which object to render
        /// </summary>
        protected DrawingObject drawingObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingObject">DrawingObjectWallpaper</param>
        public void Initialize(DrawingObject drawingObject)
        {
            this.drawingObject = drawingObject;

            this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();
        }

        /// <summary>
        /// 渲染一帧
        /// </summary>
        public abstract void DrawFrame();

        protected abstract void OnInitialize();
        protected abstract void OnRelease();
    }
}
