using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Kagura.Terminal.Controls
{
    public class VisualParagraph : List
    {
        private List<VisualLine> visualLines;

        public VisualParagraph()
        {
            this.visualLines = new List<VisualLine>();
            base.MarkerStyle = System.Windows.TextMarkerStyle.None;
            base.MarkerOffset = 0;
            base.TextAlignment = System.Windows.TextAlignment.Left;
            base.Margin = new System.Windows.Thickness(0);
            base.Padding = new System.Windows.Thickness(0);
        }

        public void CreateVisualLine()
        {
            VisualLine line = new VisualLine();
            base.ListItems.Add(line);
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