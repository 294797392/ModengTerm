using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using XTerminal.Base.Enumerations;

namespace XTerminal
{
    public class SSHAuthTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "未知";
            }

            switch ((SSHAuthTypeEnum)value)
            {
                case SSHAuthTypeEnum.None: return "不需要身份验证";
                case SSHAuthTypeEnum.Password: return "用户名和密码";
                case SSHAuthTypeEnum.PulicKey: return "公钥";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CloseSessionButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool selected = (bool)value;
            return selected ? Brushes.White : Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "未知类型";
            }

            SessionTypeEnum type = (SessionTypeEnum)value;

            switch (type)
            {
                case SessionTypeEnum.SerialPort: return "串口";
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SSH: return "SSH";
                case SessionTypeEnum.Win32CommandLine: return "命令行";

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
