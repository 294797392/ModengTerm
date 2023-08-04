using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 表示文档上的一个可视化对象（光标，文本块，文本行...）
    /// </summary>
    public abstract class DrawingObject : DrawingVisual
    {
        public string ID { get; protected set; }

        /// <summary>
        /// 和该渲染对象关联的要绘制的元素信息
        /// </summary>
        public VTDocumentElement Drawable { get; internal set; }

        public DrawingObject()
        {
        }

        protected abstract void Draw(DrawingContext dc);

        public virtual void Draw()
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
            this.Drawable = null;
            this.Offset = new Vector();
            DrawingContext dc = this.RenderOpen();
            dc.Close();
        }
    }
}
