using ModengTerm.Terminal.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace ModengTerm.ViewModels
{
    public partial class VideoTerminal
    {
        private enum DrawStateEnum
        {
            None,
            Drawing
        }

        private DrawStateEnum drawState;
        private VTDocumentGraphics drawGraphics;
        private List<VTDocumentGraphics> graphicsList;

        private void InitializeDrawing()
        {
            this.graphicsList = new List<VTDocumentGraphics>();
        }

        private VTDocumentGraphics CreateGraphics(VTDocumentElements type)
        {
            switch (type)
            {
                case VTDocumentElements.Rectangle:
                    {
                        VTDocumentRectangle rectangle = new VTDocumentRectangle()
                        {
                            ID = Guid.NewGuid().ToString(),
                        };

                        this.ActiveCanvas.CreateDrawingObject(rectangle);

                        this.graphicsList.Add(rectangle);

                        return rectangle;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public void OnDrawingMouseDown(IVideoTerminal vt, VTPoint location, int clickCount)
        {
            this.drawState = DrawStateEnum.Drawing;

            this.mouseDownPos = location;

            this.drawGraphics = this.CreateGraphics(VTDocumentElements.Rectangle);

            this.drawGraphics.OnMouseDown(location);
        }

        public void OnDrawingMouseMove(IVideoTerminal vt, VTPoint location)
        {
            if (this.drawGraphics == null)
            {
                return;
            }

            this.drawGraphics.OnMouseMove(location, this.mouseDownPos);
        }

        public void OnDrawingMouseUp(IVideoTerminal vt, VTPoint location)
        {
            this.drawState = DrawStateEnum.None;

            this.drawGraphics.OnMouseUp(location, this.mouseDownPos);

            this.drawGraphics.Position.PhysicsRow = this.ActiveDocument.FirstLine.PhysicsRow;

            this.drawGraphics = null;
        }

        public void OnDrawingMouseWheel(IVideoTerminal vt, bool upper)
        { }

        public void OnDrawingSizeChanged(IVideoTerminal vt, VTRect vtRect)
        { }
    }
}
