using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace GTerminalControl
{
    public class VisualParagraph : Paragraph
    {
        private List<VisualLine> visualLines;
        private Span span;

        public VisualParagraph()
        {
            this.visualLines = new List<VisualLine>();
            this.span = new Span();
            base.Inlines.Add(this.span);
        }

        public void CreateVisualLine()
        {
            VisualLine line = new VisualLine(this.span);
            line.CreateTextSegement();
            this.visualLines.Add(line);
        }

        public void InsertTextAtPosition(string text, int column, int row)
        {
            if (this.visualLines.Count < row)
            {
                return;
            }

            VisualLine line = this.visualLines[row];
            line.InsertTextAtPosition(text, column);
        }
    }
}