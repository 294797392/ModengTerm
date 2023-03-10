using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Document;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 表示文档上的一个可视化对象（光标，文本块，文本行...）
    /// </summary>
    public abstract class DrawingObject : DrawingVisual, IDrawingElement
    {
        /// <summary>
        /// 保存要画的对象的数据
        /// </summary>
        public VDocumentElement Data { get; set; }

        public DrawingObject()
        {
        }

        protected abstract void Draw(DrawingContext dc);

        public void Draw()
        {
            DrawingContext dc = this.RenderOpen();

            this.Draw(dc);

            dc.Close();
        }

        /// <summary>
        /// 重置此绘图对象
        /// </summary>
        public void Reset()
        {
            this.Data = null;
            this.Offset = new Vector();
            DrawingContext dc = this.RenderOpen();
            dc.Close();
        }
    }
}
