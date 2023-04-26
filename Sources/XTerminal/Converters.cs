﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using XTerminal.Session.Property;

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

            switch ((SSHAuthEnum)value)
            {
                case SSHAuthEnum.None: return "不需要身份验证";
                case SSHAuthEnum.Password: return "用户名和密码";
                case SSHAuthEnum.PulicKey: return "公钥";
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