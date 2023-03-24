using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 定义可渲染对象的类型
    /// </summary>
    public enum Drawables
    {
        /// <summary>
        /// 一个文本行
        /// </summary>
        TextLine,

        /// <summary>
        /// 光标
        /// </summary>
        Cursor,

        /// <summary>
        /// 选项
        /// </summary>
        SelectionRange
    }

    /// <summary>
    /// 表示文档里的一个可以画的元素
    /// </summary>
    public abstract class VTDrawable
    {
        private bool isDirty;

        public bool IsDirty
        {
            get { return this.isDirty; }
            protected set
            {
                if (this.isDirty != value)
                {
                    this.isDirty = value;
                }
            }
        }

        /// <summary>
        /// 该对象的类型
        /// </summary>
        public abstract Drawables Type { get; }

        /// <summary>
        /// 把该元素设置为脏元素
        /// 表示在下次渲染的时候需要重绘
        /// </summary>
        public void SetDirty(bool isDirty)
        {
            if (this.isDirty != isDirty)
            {
                this.isDirty = isDirty;
            }
        }

        /// <summary>
        /// 该数据模型对应的渲染模型
        /// </summary>
        public IDrawingObject DrawingObject { get; private set; }

        /// <summary>
        /// 关联一个Drawable对象
        /// 该操作会把drawable之前关联的文档模型取消关联
        /// </summary>
        /// <param name="drawingObject"></param>
        public void AttachDrawing(IDrawingObject drawingObject)
        {
            if (drawingObject.Drawable != null)
            {
                drawingObject.Drawable.DetachDrawing();
            }

            this.DrawingObject = drawingObject;
            this.DrawingObject.Drawable = this;
            this.SetDirty(true);
        }

        /// <summary>
        /// 取消关联的Drawable对象
        /// </summary>
        public void DetachDrawing()
        {
            if (this.DrawingObject != null)
            {
                this.DrawingObject.Drawable = null;
                this.DrawingObject = null;
            }
        }
    }
}
