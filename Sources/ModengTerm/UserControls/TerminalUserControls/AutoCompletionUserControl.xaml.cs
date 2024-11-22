using ModengTerm.Document;
using ModengTerm.Document.EventData;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Renderer;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.UserControls.Terminals
{
    /// <summary>
    /// AutoCompletionUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AutoCompletionUserControl : UserControl
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("AutoCompletionUserControl");

        private double offsetX;
        private double offsetY;


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
            DependencyProperty.Register("VideoTerminal", typeof(IVideoTerminal), typeof(AutoCompletionUserControl), new PropertyMetadata(null, VideoTerminalPropertyChangedCallback));


        public AutoCompletionUserControl()
        {
            InitializeComponent();
        }


        private void Reposition()
        {
            if (double.IsNaN(ListBoxAutoCompletionItems.ActualHeight))
            {
                return;
            }

            IVideoTerminal vt = this.VideoTerminal;
            VTDocument document = vt.ActiveDocument;
            VTCursor cursor = document.Cursor;

            double drawAreaHeight = document.Renderer.DrawAreaSize.Height;

            // 光标下面还剩余多少高度
            double remainHeight = drawAreaHeight - cursor.Bottom;

            // 显示整个列表需要的高度
            double desireHeight = ListBoxAutoCompletionItems.ActualHeight;

            double newOffsetX = 0, newOffsetY = 0;

            if (desireHeight > remainHeight)
            {
                // 剩下的区域不够显示列表。把列表显示到光标上面   
                newOffsetX = cursor.OffsetX;
                newOffsetY = cursor.OffsetY - desireHeight;
            }
            else
            {
                // 显示到光标下面
                newOffsetX = cursor.OffsetX;
                newOffsetY = cursor.Bottom;
            }

            if (newOffsetX != this.offsetX)
            {
                Canvas.SetLeft(ListBoxAutoCompletionItems, newOffsetX);
                this.offsetX = newOffsetX;
            }

            if (newOffsetY != this.offsetY)
            {
                Canvas.SetTop(ListBoxAutoCompletionItems, newOffsetY);
                this.offsetY = newOffsetY;
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

        private static void VideoTerminalPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoCompletionUserControl me = d as AutoCompletionUserControl;
            me.OnVideoTerminalPropertyChanged(e.OldValue, e.NewValue);
        }

        private void OnVideoTerminalPropertyChanged(object oldValue, object newValue)
        {
            IVideoTerminal newVt = newValue as IVideoTerminal;
            if (newVt != null)
            {
                newVt.MainDocument.Rendering += this.VTDocument_Rendering;
                newVt.AlternateDocument.Rendering += this.VTDocument_Rendering;
            }

            IVideoTerminal oldVt = oldValue as IVideoTerminal;
            if (oldVt != null) 
            {
                newVt.MainDocument.Rendering -= this.VTDocument_Rendering;
                newVt.AlternateDocument.Rendering -= this.VTDocument_Rendering;
            }
        }

        private void VTDocument_Rendering(VTDocument document, VTRenderData renderData)
        {
            if (this.IsOpen)
            {
                // 列表大小没变化，但是光标位置变化了
                // 自动完成列表要跟着光标位置移动

                this.Reposition();
            }
        }

        private void ListBoxAutoCompletionItems_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.IsOpen)
            {
                return;
            }

            // 宽度高度都没变化，不需要重新定位
            if (!e.WidthChanged && !e.HeightChanged)
            {
                return;
            }

            // SizeChanged的时候才能获取到正确的ListBox的高度
            // 所以在这个事件里去对自动完成列表重新定位
            this.Reposition();
        }

        private void ListBoxAutoCompletionItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 让选中的项目获得焦点，以便于使用方向键移动的时候滚动条可以自动滚动
            if (!this.IsOpen)
            {
                return;
            }

            ListBoxItem listBoxItem = ListBoxAutoCompletionItems.ItemContainerGenerator.ContainerFromItem(ListBoxAutoCompletionItems.SelectedItem) as ListBoxItem;
            if (listBoxItem == null)
            {
                return;
            }

            if (!listBoxItem.IsFocused)
            {
                // ListBoxItem获取焦点后，可以使用方向键选中不同的项目
                // 同时ListBoxItem也会触发KeyDown事件
                // 因为KeyDown事件是路由事件，会传播到AutoCompletionUserControl
                // ShellSessionUserControl再注册AutoCompletionUserControl的KeyDown事件作为终端的输入事件
                listBoxItem.Focus();
            }
        }
    }
}
