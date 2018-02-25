using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    /// <summary>
    /// 表示一行里的一个元素
    /// 元素可以是一个普通文本，也可以是一个可以打开的连接
    /// </summary>
    public class TextItem : FormattedText
    {
        public TextItem(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground) :
            base(textToFormat, culture, flowDirection, typeface, emSize, foreground)
        { }

        public TextItem(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution) :
            base(textToFormat, culture, flowDirection, typeface, emSize, foreground, numberSubstitution)
        { }

        public TextItem(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double emSize, Brush foreground, NumberSubstitution numberSubstitution, TextFormattingMode textFormattingMode) :
            base(textToFormat, culture, flowDirection, typeface, emSize, foreground, numberSubstitution, textFormattingMode)
        { }
    }
}