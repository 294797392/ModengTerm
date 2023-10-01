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
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDocumentRectangle");

        public override VTDocumentElements Type => VTDocumentElements.Rectangle;

        private double width;
        private double height;

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

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor { get; set; }

        /// <summary>
        /// 边框粗细
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string Background { get; set; }

        public override void OnMouseDown(VTPoint location)
        {
            this.OffsetX = location.X;
            this.OffsetY = location.Y;
        }

        public override void OnMouseMove(VTPoint location, VTPoint mouseDown)
        {
            this.Width = Math.Abs(mouseDown.X - location.X);
            this.Height = Math.Abs(mouseDown.Y - location.Y);

            this.OffsetX = Math.Min(location.X, mouseDown.X);
            this.OffsetY = Math.Min(location.Y, mouseDown.Y);

            this.RequestInvalidate();
        }

        public override void OnMouseUp(VTPoint location, VTPoint mouseDown)
        {
        }
    }
}
