using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm
{
    public static class MMessageBox
    {
        public static void Info(string format, params object[] args)
        {
            string message = string.Format(format, args);
            MessageBox.Show(message, "信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Error(string format, params object[] args)
        {
            string message = string.Format(format, args);
            MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
