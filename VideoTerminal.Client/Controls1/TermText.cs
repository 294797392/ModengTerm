using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XTerminal.Controls
{
    public class TermText : FormattedText
    {
        public TermText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground) :
            base(textToFormat, culture, flowDirection, typeface, emSize, foreground)
        { }

        public TermText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution) :
            base(textToFormat, culture, flowDirection, typeface, emSize, foreground, numberSubstitution)
        { }

        public TermText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution, TextFormattingMode textFormattingMode) :
            base(textToFormat, culture, flowDirection, typeface, emSize, foreground, numberSubstitution, textFormattingMode)
        { }
    }
}