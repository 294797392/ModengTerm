using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XTerminal.Client.TerminalConsole
{
    public class TerminalText : FormattedText
    {
        #region 构造方法

        public TerminalText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground)
            : base(textToFormat, culture, flowDirection, typeface, emSize, foreground)
        {

        }
        public TerminalText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution)
            : base(textToFormat, culture, flowDirection, typeface, emSize, foreground, numberSubstitution)
        { }

        public TerminalText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution, TextFormattingMode textFormattingMode)
            : base(textToFormat, culture, flowDirection, typeface, emSize, foreground, numberSubstitution, textFormattingMode)
        { }

        #endregion
    }
}