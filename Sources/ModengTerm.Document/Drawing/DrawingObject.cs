using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    /// <summary>
    /// 表示文档上的一个可视化对象（光标，文本块，文本行...）
    /// </summary>
    public abstract class DrawingObject : DrawingVisual, IDrawingObject
    {
        #region 实例变量

        private Visibility visible = Visibility.Visible;

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

        public void Initialize()
        {
            OnInitialize();
        }

        /// <summary>
        /// 绘制图像
        /// </summary>
        public virtual void Draw()
        {
            DrawingContext dc = RenderOpen();

            OnDraw(dc);

            dc.Close();
        }

        public void Release()
        {
            OnRelease();
        }


        public void SetOpacity(double opacity)
        {
            if (Opacity == opacity)
            {
                return;
            }

            Opacity = opacity;
        }

        public void Arrange(double x, double y)
        {
            Offset = new Vector(x, y);
        }

        #endregion
    }
}
