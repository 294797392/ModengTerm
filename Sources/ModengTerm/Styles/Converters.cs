using ModengTerm.Base;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFToolkit.MVVM;

namespace ModengTerm.Styles
{
    public class TreeViewItemIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TreeViewItem treeViewItem = value as TreeViewItem;
            if (treeViewItem == null) 
            {
                return 0;
            }

            TreeNodeViewModel treeNodeViewModel = treeViewItem.DataContext as TreeNodeViewModel;
            if (treeNodeViewModel == null) 
            {
                return 0;
            }

            return new Thickness(treeNodeViewModel.Level * MTermConsts.TreeViewItemIndent, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MenuItemHeaderBottomBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double actualWidth = (double)value;
            return actualWidth - 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
