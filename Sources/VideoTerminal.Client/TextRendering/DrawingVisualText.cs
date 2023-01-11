using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace VideoTerminal.TextRendering
{
    public class DrawingVisualText : DrawingVisual
    {
        private TextFormatter textFormatter;
        private DefaultTextSource textSource;
        private DefaultTextParagraphProperties textParagraphProperties;

        public string Text { get; set; }

        public DrawingVisualText()
        {
            this.textFormatter = TextFormatter.Create(TextFormattingMode.Display);
            this.textSource = new DefaultTextSource();
            this.textParagraphProperties = new DefaultTextParagraphProperties();
        }

        public void Render()
        {
            this.textSource.Text = this.Text;
            int textSourcePosition = 0;
            Point lineOrigin = new Point(0, 0);

            using (DrawingContext dc = base.RenderOpen())
            {
                while (textSourcePosition < this.textSource.Text.Length)
                {
                    using (TextLine textLine = this.textFormatter.FormatLine(this.textSource, textSourcePosition, 10, this.textParagraphProperties, null))
                    {
                        textLine.Draw(dc, lineOrigin, InvertAxes.None);

                        // 更新下一次要渲染的字符的索引
                        textSourcePosition += textLine.Length;

                        // 更新下一次要渲染的字符的位置
                        lineOrigin.X += textLine.Width;
                        //lineOrigin.Y += textLine.Height;
                    }
                }
            }
        }
    }
}
