using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Enumerations;

namespace ModengTerm
{
    public static class MTermUtils
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("ModengTermUtils");

        private static readonly BrushConverter brushConverter = new BrushConverter();

        public static Brush GetBrush(string colorName)
        {
            try
            {
                return (Brush)brushConverter.ConvertFrom(colorName);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("colorName转换Brush异常, {0}, {1}", colorName, ex);
                return Brushes.Transparent;
            }
        }
    }
}
