using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media.TextFormatting;

namespace VideoTerminal.TextRendering
{
    public class DefaultTextSource : TextSource
    {
        public string Text { get; set; }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            //CharacterBufferRange cbr = new CharacterBufferRange();
            //return new TextSpan<CultureSpecificCharacterBufferRange>(textSourceCharacterIndexLimit, new CultureSpecificCharacterBufferRange(CultureInfo.CurrentCulture, cbr));
            throw new NotImplementedException();
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotImplementedException();
        }

        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            if (textSourceCharacterIndex >= this.Text.Length)
            {
                return new TextEndOfParagraph(1);
            }

            DefaultTextRunProperties textRunProperties = new DefaultTextRunProperties();
            return new TextCharacters(this.Text, textSourceCharacterIndex, this.Text.Length - textSourceCharacterIndex, textRunProperties);
        }
    }
}