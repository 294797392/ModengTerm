using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using XTerminal.Client.TerminalConsole.Rendering;

namespace XTerminal.UnitTest
{
    /// <summary>
    /// TestTextRenderingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestTextRenderingWindow : Window
    {
        public TestTextRenderingWindow()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            TextFormatter formatter = TextFormatter.Create();
            CustomTextSource textSource = new CustomTextSource();
            CustomTextParagraphProperties paragraphProperties = new CustomTextParagraphProperties();
            System.Windows.Media.TextFormatting.TextLine line = formatter.FormatLine(textSource, 0, 100, paragraphProperties, null);
            line.Draw(drawingContext, new Point(0, 0), InvertAxes.None);

            base.OnRender(drawingContext);
        }
    }

    public class CustomTextSource : TextSource
    {
        public string Text = "sdasdadasdsadsdd";

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            CharacterBufferRange cbr = new CharacterBufferRange(Text, 0, Text.Length);
            return new TextSpan<CultureSpecificCharacterBufferRange>(
             textSourceCharacterIndexLimit,
             new CultureSpecificCharacterBufferRange(CultureInfo.CurrentUICulture, cbr)
             );
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotImplementedException();
        }

        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            if (textSourceCharacterIndex >= Text.Length)
            {
                return new TextEndOfParagraph(1);
            }

            return new TextCharacters(Text, textSourceCharacterIndex, Text.Length - textSourceCharacterIndex, new CustomTextRunProperties());
        }
    }

    public class CustomTextRunProperties : TextRunProperties
    {
        public override Brush BackgroundBrush
        {
            get
            {
                return Brushes.Yellow;
            }
        }

        public override CultureInfo CultureInfo
        {
            get
            {
                return CultureInfo.CurrentCulture;
            }
        }

        public override double FontHintingEmSize
        {
            get
            {
                return 12;
            }
        }

        public override double FontRenderingEmSize
        {
            get
            {
                return 12;
            }
        }

        public override Brush ForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public override TextDecorationCollection TextDecorations
        {
            get
            {
                return null;
            }
        }

        public override TextEffectCollection TextEffects
        {
            get
            {
                return null;
            }
        }

        public override Typeface Typeface
        {
            get
            {
                return new Typeface(new FontFamily("宋体"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            }
        }
    }

    public class CustomTextParagraphProperties : TextParagraphProperties
    {
        internal TextRunProperties defaultTextRunProperties = new CustomTextRunProperties();
        internal TextWrapping textWrapping;
        internal double tabSize;
        internal double indent;
        internal bool firstLineInParagraph;

        public override double DefaultIncrementalTab
        {
            get { return tabSize; }
        }

        public override FlowDirection FlowDirection { get { return FlowDirection.LeftToRight; } }
        public override TextAlignment TextAlignment { get { return TextAlignment.Left; } }
        public override double LineHeight { get { return double.NaN; } }
        public override bool FirstLineInParagraph { get { return firstLineInParagraph; } }
        public override TextRunProperties DefaultTextRunProperties { get { return defaultTextRunProperties; } }
        public override TextWrapping TextWrapping { get { return textWrapping; } }
        public override TextMarkerProperties TextMarkerProperties { get { return null; } }
        public override double Indent { get { return indent; } }
    }

}