using ModengTerm.Base.Enumerations.Terminal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModengTerm.OfficialAddons.Snippet
{
    public class SnippetType2TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SnippetTypes snippetType = (SnippetTypes)value;

            switch (snippetType)
            {
                case SnippetTypes.Text: return "纯文本";
                case SnippetTypes.Hex: return "十六进制数据";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
