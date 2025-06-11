using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
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
using System.Windows.Shapes;
using XTerminal;

namespace ModengTerm.OfficialAddons.Find
{
    /// <summary>
    /// 搜索窗口只能显示一个，用这个类来管理搜索窗口的生命周期
    /// </summary>
    public static class FindWindowMgr
    {
        private static FindWindow findWindow;

        /// <summary>
        /// 获取查找窗口是否显示
        /// </summary>
        public static bool WindowShown { get { return findWindow != null; } }

        /// <summary>
        /// 显示搜索窗口
        /// 如果已经显示了搜索窗口，那么使用传入的shellSession立即进行一次搜索
        /// </summary>
        /// <param name="shellSession">要搜索的会话ViewModel</param>
        public static void Show()
        {
            throw new RefactorImplementedException();

            //XTermSession session = shellSession.Session;
            //IVideoTerminal vt = shellSession.VideoTerminal;

            //string highlightBackground = session.GetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_BACKCOLOR);
            //string highlightForeground = session.GetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_FONTCOLOR);

            //FindVM findVM = null;

            //if (findWindow == null)
            //{
            //    findVM = new FindVM()
            //    {
            //        HighlightBackground = VTColor.CreateFromRgbKey(highlightBackground),
            //        HighlightForeground = VTColor.CreateFromRgbKey(highlightForeground)
            //    };

            //    findWindow = new FindWindow(findVM);
            //    findWindow.Owner = Application.Current.MainWindow;
            //    findWindow.Closed += FindWindow_Closed;
            //    findWindow.Show();
            //}
            //else
            //{
            //    // 已经显示了搜索窗口
            //    findVM = findWindow.DataContext as FindVM;
            //}

            //findVM.SetVideoTerminal(vt);
        }

        private static void FindWindow_Closed(object? sender, EventArgs e)
        {
            findWindow.Closed -= FindWindow_Closed;

            FindVM findVM = findWindow.DataContext as FindVM;
            findVM.Release();

            findWindow = null;
        }
    }

    /// <summary>
    /// FindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindWindow : MdWindow
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindWindow");

        private FindVM viewModel;
        private bool finding;

        public FindWindow(FindVM vm)
        {
            InitializeComponent();

            this.InitializeWindow(vm);
        }

        private void InitializeWindow(FindVM vm)
        {
            this.viewModel = vm;
            base.DataContext = vm;
        }

        private void FindAsync(bool findOnce)
        {
            if (this.finding)
            {
                return;
            }

            this.viewModel.FindAll = findOnce;

            //Task.Factory.StartNew(() =>
            //{
            //    this.finding = true;

            //    try
            //    {
            //        this.viewModel.FindNext();
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Error("查找异常", ex);
            //    }
            //    finally
            //    {
            //        this.finding = false;
            //    }
            //});
        }

        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            this.FindAsync(true);
        }

        private void ButtonFindAll_Click(object sender, RoutedEventArgs e)
        {
            this.FindAsync(false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.viewModel.Release();
        }

        private void ButtonNextMatches_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
