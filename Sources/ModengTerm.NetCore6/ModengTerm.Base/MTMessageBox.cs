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
    }
}
