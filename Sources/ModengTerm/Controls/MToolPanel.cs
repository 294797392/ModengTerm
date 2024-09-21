using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.Controls
{
    public class MToolPanelItem : ViewModelBase
    {

    }

    public class MToolPanel : Control
    {
        public BindableCollection<MToolPanelItem> Items
        {
            get { return (BindableCollection<MToolPanelItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(BindableCollection<MToolPanelItem>), typeof(MToolPanel), new PropertyMetadata(null));


    }
}
