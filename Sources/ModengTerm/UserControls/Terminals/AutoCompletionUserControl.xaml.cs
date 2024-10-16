using ModengTerm.Document;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.MVVM;

namespace ModengTerm.UserControls.Terminals
{
    /// <summary>
    /// AutoCompletionUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AutoCompletionUserControl : UserControl
    {
        public BindableCollection<string> Items
        {
            get { return (BindableCollection<string>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(BindableCollection<string>), typeof(AutoCompletionUserControl), new PropertyMetadata(null, ItemsPropertyChangedCallback));


        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(AutoCompletionUserControl), new PropertyMetadata(false, IsOpenPropertyChangedCallback));


        public IVideoTerminal VideoTerminal
        {
            get { return (IVideoTerminal)GetValue(VideoTerminalProperty); }
            set { SetValue(VideoTerminalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VideoTerminal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoTerminalProperty =
            DependencyProperty.Register("VideoTerminal", typeof(IVideoTerminal), typeof(AutoCompletionUserControl), new PropertyMetadata(null));


        public AutoCompletionUserControl()
        {
            InitializeComponent();
        }


        private static void ItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompletionUserControl me = d as AutoCompletionUserControl;
            me.OnItemsPropertyChanged(e.OldValue, e.NewValue);
        }

        private void OnItemsPropertyChanged(object oldValue, object newValue) 
        {
            ListBoxAutoCompletionItems.DataContext = newValue;
        }


        private static void IsOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompletionUserControl me = d as AutoCompletionUserControl;
            me.OnIsOpenPropertyChanged(e.OldValue, e.NewValue);
        }

        private void OnIsOpenPropertyChanged(object oldValue, object newValue) 
        {
            bool isOpen = (bool)newValue;

            if (isOpen)
            {
                IVideoTerminal vt = this.VideoTerminal;
                VTDocument document = vt.ActiveDocument;
                VTCursor cursor = document.Cursor;

                Canvas.SetLeft(ListBoxAutoCompletionItems, cursor.OffsetX);
                Canvas.SetTop(ListBoxAutoCompletionItems, cursor.OffsetY + vt.Typeface.Height);

                base.Visibility = Visibility.Visible;
            }
            else
            {
                base.Visibility = Visibility.Collapsed;
            }
        }
    }
}

