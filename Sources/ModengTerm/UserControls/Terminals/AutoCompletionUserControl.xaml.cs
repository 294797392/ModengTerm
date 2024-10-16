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
using System.Windows.Media.Animation;
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
        private static log4net.ILog logger = log4net.LogManager.GetLogger("AutoCompletionUserControl");

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


        private void Reposition()
        {
            IVideoTerminal vt = this.VideoTerminal;
            VTDocument document = vt.ActiveDocument;
            VTCursor cursor = document.Cursor;

            double drawAreaHeight = document.Renderer.DrawAreaSize.Height;

            // 光标下面还剩余多少高度
            double remainHeight = drawAreaHeight - cursor.Bottom;

            // 显示整个列表需要的高度
            double desireHeight = ListBoxAutoCompletionItems.ActualHeight;

            if (desireHeight > remainHeight)
            {
                // 剩下的区域不够显示列表。把列表显示到光标上面   
                Canvas.SetLeft(ListBoxAutoCompletionItems, cursor.OffsetX);
                Canvas.SetTop(ListBoxAutoCompletionItems, cursor.OffsetY - desireHeight);
            }
            else
            {
                // 显示到光标下面
                Canvas.SetLeft(ListBoxAutoCompletionItems, cursor.OffsetX);
                Canvas.SetTop(ListBoxAutoCompletionItems, cursor.Bottom);
            }
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
                base.Visibility = Visibility.Visible;
            }
            else
            {
                base.Visibility = Visibility.Collapsed;
            }
        }

        private void ListBoxAutoCompletionItems_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.IsOpen)
            {
                return;
            }

            // SizeChanged的时候才能获取到正确的ListBox的高度
            // 所以在这个事件里去对自动完成列表重新定位
            this.Reposition();
        }
    }
}
