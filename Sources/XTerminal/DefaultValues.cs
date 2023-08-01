using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace XTerminal
{
    public static class DefaultValues
    {
        /// <summary>
        /// 默认的每一行的间距
        /// </summary>
        public static Thickness LineMargin = new Thickness(0, 1, 0, 1);

        public static double CaretWidth = SystemParameters.CaretWidth;
        public static double CaretHeight = 12;
        public static Brush CaretBrush = Brushes.Red;

        public static readonly FontFamily FontFamily = new FontFamily("宋体");
        public static readonly FontWeight FontWeight = FontWeights.Normal;
        public static readonly FontStyle FontStyle = FontStyles.Normal;
        public static readonly FontStretch FontStretch = FontStretches.Normal;
    }
}
