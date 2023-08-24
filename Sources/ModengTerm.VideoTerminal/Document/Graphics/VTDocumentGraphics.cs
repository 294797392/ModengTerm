using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document.Graphics
{
    public abstract class VTDocumentGraphics : VTDocumentElement
    {
        private bool isRenderDirty;

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
    }
}
