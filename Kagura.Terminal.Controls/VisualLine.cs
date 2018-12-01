using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Kagura.Terminal.Controls
{
    public class VisualLine : ListItem
    {
        private Run textRun;
        private Paragraph container;

        public VisualLine()
        {
            this.textRun = new Run();
            this.container = new Paragraph();
            this.container.Inlines.Add(this.textRun);
            base.Blocks.Add(this.container);
        }

        public void InsertTextAtPosition(string text, int column)
        {
            string curText = this.textRun.Text;
            string start = curText.Substring(0, column);
            string end = curText.Substring(column, curText.Length - column);
            this.textRun.Text = string.Format("{0}{1}{2}", start, text, end);
        }
    }
}