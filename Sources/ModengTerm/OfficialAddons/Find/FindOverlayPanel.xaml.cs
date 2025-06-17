using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
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

namespace ModengTerm.OfficialAddons.Find
{
    /// <summary>
    /// FindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindOverlayPanel : UserControl, IOverlayPanelCallback
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindOverlayPanel");

        private FindVM viewModel;
        private bool finding;

        public FindOverlayPanel()
        {
            InitializeComponent();
        }

        private void InitializeWindow()
        {
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

        #region 事件处理器

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

        private void TextBoxKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        #endregion

        #region IOverlayPanelCallback

        public IShellTab OwnerTab { get; set; }

        public HostFactory HostFactory { get; set; }

        public void OnInitialize()
        {
        }

        public void OnRelease()
        {
        }

        public void OnLoaded()
        {
        }

        public void OnUnload()
        {
        }

        #endregion
    }
}
