using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Controls
{
    /// <summary>
    /// 参考：
    /// https://docs.microsoft.com/zh-cn/dotnet/desktop/wpf/advanced/advanced-text-formatting?view=netframeworkdesktop-4.8&viewFallbackFrom=netframework-4.7.1#
    /// </summary>
    public class TerminalTextSource : TextSource
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

            TerminalTextRunProperties textRunProperties = new TerminalTextRunProperties();
            return new TextCharacters(this.Text, textSourceCharacterIndex, this.Text.Length - textSourceCharacterIndex, textRunProperties);
        }
    }
}