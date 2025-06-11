using ModengTerm.Base.Enumerations.Terminal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModengTerm.Addon.Official.QuickInput
{
    public class ShellCommandTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CommandTypeEnum))
            {
                return string.Empty;
            }

            CommandTypeEnum commandType = (CommandTypeEnum)value;

            switch (commandType)
            {
                case CommandTypeEnum.PureText: return "纯文本";
                case CommandTypeEnum.HexData: return "十六进制数据";
                default: throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
