using ModengTerm.Base;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.ViewModels.Terminals
{
    public class MTermSessionVM : InputSessionVM
    {
        private VTDocument document;

        public IDrawingDocument DrawingDocument { get; set; }

        public MTermSessionVM() : 
            base(null)
        { 
        }

        protected override int OnOpen()
        {
            VTDocumentOptions options = new VTDocumentOptions() 
            {
                DrawingObject = this.DrawingDocument
            };

            this.document = new VTDocument(options);
            this.document.Initialize();

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            this.document.Release();
        }

        public override void SendInput(UserInput userInput)
        {
        }
    }
}
