using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Base
{
    public static class MTMessageBox
    {
        public static void Info(string message, params object[] args)
        {
            MessageBox.Show(string.Format(message, args), "信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Error(string message, params object[] args)
        {
            MessageBox.Show(string.Format(message, args), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool Confirm(string message, params object[] args)
        {
            return MessageBox.Show(string.Format(message, args), "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
