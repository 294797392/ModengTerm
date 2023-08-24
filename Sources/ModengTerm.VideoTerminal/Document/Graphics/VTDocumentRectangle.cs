using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document.Graphics
{
    public class VTDocumentRectangle : VTDocumentGraphics
    {
        public override VTDocumentElements Type => VTDocumentElements.Rectangle;

        private double width;
        private double height;
        private double x;
        private double y;

        public double X
        {
            get { return this.x; }
            set 
            {
                if (this.x != value)
                {
                    this.x = value;
                    this.SetRenderDirty(true);
                }
            }
        }

        public double Y
        {
            get { return this.x; }
            set
            {
                if (this.y != value)
                {
                    this.y = value;
                    this.SetRenderDirty(true);
                }
            }
        }

        public double Width
        {
            get { return this.width; }
            set
            {
                if (this.width != value)
                {
                    this.width = value;
                    this.SetRenderDirty(true);
                }
            }
        }

        public double Height
        {
            get { return this.height; }
            set
            {
                if (this.height != value)
                {
                    this.height = value;
                    this.SetRenderDirty(true);
                }
            }
        }
    }
}
