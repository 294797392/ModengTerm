using ModengTerm.Terminal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    public class VTWallpaper : VTFramedElement
    {
        private VTRect vtc;
        private bool dirty;

        public Wallpaper Wallpaper { get; set; }

        public VTRect Rect
        {
            get { return this.vtc; }
            set
            {
                this.vtc = value;
                this.SetDirty(true);
            }
        }

        public override VTDocumentElements Type => VTDocumentElements.Wallpaper;

        private void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;
            }
        }

        public override void RequestInvalidate()
        {
            if (this.dirty)
            {
                this.DrawingObject.Draw();

                this.dirty = false;
            }
        }
    }
}
