using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace GTerminalControl
{
    public class VisualLine
    {
        private Span span;
        private List<Run> textSegements;

        public VisualLine(Span span)
        {
            this.textSegements = new List<Run>();
            this.span = span;
        }

        public void CreateTextSegement()
        {
            Run run = new Run();
            this.span.Inlines.Add(run);
            this.textSegements.Add(run);
        }

        public void InsertTextAtPosition(string text, int column)
        {
            string curText = this.textSegements.Last().Text;
            string start = curText.Substring(0, column);
            string end = curText.Substring(column, curText.Length - column);
            this.textSegements.Last().Text = string.Format("{0}{1}{2}", start, text, end);
        }
    }
}