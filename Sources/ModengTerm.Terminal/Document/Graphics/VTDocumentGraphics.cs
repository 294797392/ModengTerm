using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document.Graphics
{
    public abstract class VTDocumentGraphics : VTDocumentElement
    {
        protected bool isRenderDirty;

        #region 属性

        /// <summary>
        /// 唯一编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 描述该图形的位置
        /// </summary>
        public int PhysicsRow { get; set; }

        #endregion

        #region 构造方法

        public VTDocumentGraphics()
        {
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
                this.DrawingObject.Draw();

                this.isRenderDirty = false;
            }

            if (this.arrangeDirty)
            {
                this.DrawingObject.Arrange(this.OffsetX, this.OffsetY);

                this.arrangeDirty = false;
            }
        }

        public abstract void OnMouseDown(VTPoint location);
        public abstract void OnMouseMove(VTPoint location, VTPoint mouseDown);
        public abstract void OnMouseUp(VTPoint location, VTPoint mouseDown);
    }
}
