using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    public class TerminalLineTextSource : TextSource
    {
        internal List<TerminalLineElement> lineElements = null;

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            CharacterBufferRange cbr = new CharacterBufferRange();
            return new TextSpan<CultureSpecificCharacterBufferRange>(textSourceCharacterIndexLimit, new CultureSpecificCharacterBufferRange(CultureInfo.CurrentCulture, cbr));
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotImplementedException();
        }

        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            if (this.lineElements == null)
            {
                return new TextEndOfParagraph(1);
            }

            foreach (TerminalLineElement element in this.lineElements)
            {
                if (textSourceCharacterIndex >= element.ColumnIndex)
                {
                    return element.CreateTextRun();
                }
            }

            return new TextEndOfParagraph(1);
        }
    }
}