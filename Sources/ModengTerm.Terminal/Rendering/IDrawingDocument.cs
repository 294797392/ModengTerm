using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Rendering
{
    /// <summary>
    /// 表示一个用来渲染终端输出的表面
    /// </summary>
    public interface IDrawingDocument
    {
        /// <summary>
        /// 创建一个渲染对象
        /// </summary>
        /// <returns>绘图对象</returns>
        IDrawingObject CreateDrawingObject(VTDocumentElement element);

        /// <summary>
        /// 删除并释放渲染对象
        /// </summary>
        void DeleteDrawingObject(IDrawingObject drawingObject);

        /// <summary>
        /// 删除所有的渲染对象
        /// </summary>
        void DeleteDrawingObjects();
    }
}
