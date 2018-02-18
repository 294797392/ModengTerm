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
    public class LinuxTitleTextRunProperties : TextRunProperties
    {
        private Typeface typeface;

        public override Brush BackgroundBrush { get { return Brushes.Transparent; } }

        public override CultureInfo CultureInfo { get { return CultureInfo.CurrentCulture; } }

        /// <summary>
        /// 获取文本大小（以磅为单位），然后将其用于字体提示。
        /// 一个表示文本大小（以磅为单位）的 System.Double。默认值为 12 磅。
        /// </summary>
        public override double FontHintingEmSize { get { return 12; } }

        /// <summary>
        /// 获取文本运行的文本大小（以磅为单位）。
        /// 一个以 DIP（与设备无关的像素）为单位表示文本大小的 System.Double。默认值为 12 DIP。
        /// </summary>
        public override double FontRenderingEmSize { get { return 12; } }

        public override Brush ForegroundBrush { get { return Brushes.Black; } }

        public override TextDecorationCollection TextDecorations { get { return null; } }

        public override TextEffectCollection TextEffects { get { return null; } }

        public override Typeface Typeface { get { return this.typeface; } }

        public LinuxTitleTextRunProperties()
        {
            FontFamily fontFamily = new FontFamily("宋体");
            this.typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        }
    }
}