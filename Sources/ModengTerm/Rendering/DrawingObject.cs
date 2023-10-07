using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 表示文档上的一个可视化对象（光标，文本块，文本行...）
    /// </summary>
    public abstract class DrawingObject : DrawingVisual, IDrawingObject
    {
        #region 实例变量

        private bool visible = true;
        protected VTDocumentElement documentElement;

        #endregion

        #region 属性

        public string ID { get; protected set; }

        #endregion

        #region 构造方法

        public DrawingObject()
        {
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// DrawingObject初始化完成之后调用
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// DrawingObjectRelease完之后调用
        /// </summary>
        protected abstract void OnRelease();

        /// <summary>
        /// 由DrawingObject.Draw方法调用
        /// </summary>
        /// <param name="dc"></param>
        protected abstract void OnDraw(DrawingContext dc);

        #endregion

        #region 公开接口

        public void Initialize(VTDocumentElement documentElement)
        {
            this.documentElement = documentElement;

            this.OnInitialize();
        }

        /// <summary>
        /// 绘制图像
        /// </summary>
        public virtual void Draw()
        {
            DrawingContext dc = this.RenderOpen();

            this.OnDraw(dc);

            dc.Close();
        }

        public void Release()
        {
            this.OnRelease();
        }


        public void SetOpacity(double opacity)
        {
            this.Opacity = opacity;
        }

        public void Arrange(double x, double y)
        {
            this.Offset = new Vector(x, y);
        }

        public void SetVisible(bool visible)
        {
            if (this.visible == visible)
            {
                return;
            }

            if (visible)
            {
                this.Opacity = 1;
            }
            else
            {
                this.Opacity = 0;
            }

            this.visible = visible;
        }

        #endregion
    }
}
