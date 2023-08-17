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
        #region 属性

        public string ID { get; protected set; }

        /// <summary>
        /// 和该渲染对象关联的渲染模型的信息
        /// </summary>
        public VTDocumentElement DocumentElement { get; private set; }

        #endregion
        
        #region 构造方法

        public DrawingObject()
        {
        }

        #endregion

        #region 受保护方法

        protected abstract void Draw(DrawingContext dc);

        protected abstract void OnInitialize(VTDocumentElement element);

        protected virtual void OnRelease()
        { }

        #endregion

        #region 公开接口

        public void Initialize(VTDocumentElement element)
        {
            this.DocumentElement = element;

            this.OnInitialize(element);
        }

        public virtual void Draw()
        {
            DrawingContext dc = this.RenderOpen();

            this.Draw(dc);

            dc.Close();
        }

        public void Release()
        {
            this.OnRelease();

            this.DocumentElement = null;
        }

        #endregion
    }
}
