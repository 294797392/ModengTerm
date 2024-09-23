using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFToolkit.MVVM;

namespace ModengTerm.Controls
{
    [TemplatePart(Name = "PART_ItemsHeader", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(MTermButton))]
    public class MToolPanel : Control
    {
        private ContentControl contentControl;
        private ListBox listBox;
        private MTermButton buttonClose;


        public MenuVM Menu
        {
            get { return (MenuVM)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register("Menu", typeof(MenuVM), typeof(MToolPanel), new PropertyMetadata(null, MenuPropertyChangedCallback));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.contentControl = this.Template.FindName("PART_Content", this) as ContentControl;
            this.listBox = this.Template.FindName("PART_ItemsHeader", this) as ListBox;
            this.listBox.SelectionChanged += ListBox_SelectionChanged;

            this.buttonClose = this.Template.FindName("PART_CloseButton", this) as MTermButton;
            this.buttonClose.Click += ButtonClose_Click;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.Visibility = Visibility.Collapsed;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Menu.InvokeWhenSelectionChanged();
        }


        private void OnMenuPropertyChanged(MenuVM oldValue, MenuVM newValue)
        {
            this.contentControl.DataContext = newValue;
        }

        private static void MenuPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MToolPanel me = d as MToolPanel;
            me.OnMenuPropertyChanged(e.OldValue as MenuVM, e.NewValue as MenuVM);
        }
    }
}
