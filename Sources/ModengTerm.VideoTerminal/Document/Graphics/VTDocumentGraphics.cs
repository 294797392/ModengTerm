using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document.Graphics
{
    public class VTGraphicsPosition
    {
        public int PhysicsRow { get; set; }

        /// <summary>
        /// 相对于PhysicsRow的X偏移量
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// 相对于PhysicsRow的Y偏移量
        /// </summary>
        public double OffsetY { get; set; }
    }

    public abstract class VTDocumentGraphics : VTDocumentElement
    {
        private bool isRenderDirty;

        #region 属性

        /// <summary>
        /// 唯一编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 描述该图形的位置
        /// </summary>
        public VTGraphicsPosition Position { get; private set; }

        #endregion

        #region 构造方法

        public VTDocumentGraphics()
        {
            this.Position = new VTGraphicsPosition();
        }

        #endregion

        protected void SetRenderDirty(bool dirty)
        {
            if (this.isRenderDirty != dirty)
            {
                this.isRenderDirty = dirty;
            }
        }

        public override void RequestInvalidate()
        {
            if (this.isRenderDirty)
            {
                this.DrawingContext.Draw();

                this.isRenderDirty = false;
            }

            if (this.arrangeDirty)
            {
                this.DrawingContext.Arrange(this.OffsetX, this.OffsetY);

                this.arrangeDirty = false;
            }
        }

        public abstract void OnMouseDown(VTPoint location);
        public abstract void OnMouseMove(VTPoint location, VTPoint mouseDown);
        public abstract void OnMouseUp(VTPoint location, VTPoint mouseDown);
    }
}
