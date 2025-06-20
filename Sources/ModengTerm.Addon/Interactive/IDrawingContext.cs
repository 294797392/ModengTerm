using ModengTerm.Document;
using ModengTerm.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 公开终端的绘图接口
    /// </summary>
    public interface IDrawingContext
    {
        /// <summary>
        /// 创建一个绘图对象
        /// </summary>
        /// <returns></returns>
        GraphicsObject CreateGraphicsObject();

        /// <summary>
        /// 删除一个绘图对象
        /// </summary>
        /// <param name="drawingObject"></param>
        void DeleteGraphicsObject(GraphicsObject graphicsObject);
    }
}