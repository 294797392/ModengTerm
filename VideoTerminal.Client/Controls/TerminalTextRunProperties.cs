using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Controls
{
    public class TerminalTextRunProperties : TextRunProperties
    {
        #region 实例变量

        private Typeface typeface;
        private double fontRenderingEmSize;
        private double fontHintingEmSize;
        private TextDecorationCollection textDecorations;
        private Brush foregroundBrush;
        private Brush backgroundBrush;
        private CultureInfo cultureInfo;
        private TextEffectCollection textEffects;

        #endregion

        #region 属性

        public override Typeface Typeface { get { return this.typeface; } }

        public override double FontRenderingEmSize { get { return this.fontRenderingEmSize; } }

        public override double FontHintingEmSize { get { return this.fontHintingEmSize; } }

        public override TextDecorationCollection TextDecorations { get { return this.textDecorations; } }

        public override Brush ForegroundBrush { get { return this.foregroundBrush; } }

        public override Brush BackgroundBrush { get { return this.backgroundBrush; } }

        public override CultureInfo CultureInfo { get { return this.cultureInfo; } }

        public override TextEffectCollection TextEffects { get { return this.textEffects; } }

        #endregion

        #region 构造方法

        public TerminalTextRunProperties()
        {
            this.typeface = new Typeface("Arial");
            this.fontRenderingEmSize = 12;
            this.fontHintingEmSize = 12;
            this.textDecorations = new TextDecorationCollection();
            this.foregroundBrush = Brushes.Black;
            this.backgroundBrush = Brushes.Transparent;
            this.cultureInfo = CultureInfo.CurrentCulture;
            this.textEffects = new TextEffectCollection();
        }

        #endregion
    }
}
