using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Ftp
{
    /// <summary>
    /// 导航栏ViewModel
    /// </summary>
    public class AddressbarVM : ViewModelBase
    {
        private BitmapSource directoryIcon;

        /// <summary>
        /// 当前显示的目录的图标
        /// </summary>
        public BitmapSource DirectoryIcon
        {
            get { return this.directoryIcon; }
            set
            {
                if (this.directoryIcon != value)
                {
                    this.directoryIcon = value;
                    this.NotifyPropertyChanged("DirectoryIcon");
                }
            }
        }

        /// <summary>
        /// 当前显示的目录链
        /// </summary>
        public BindableCollection<DirectoryVM> DirectroyChain { get; private set; }

        /// <summary>
        /// 在Popup窗口中预览的目录列表
        /// </summary>
        public BindableCollection<DirectoryVM> PreviewDirectories { get; private set; }

        public AddressbarVM()
        {
            this.DirectroyChain = new BindableCollection<DirectoryVM>();
            this.PreviewDirectories = new BindableCollection<DirectoryVM>();
        }
    }
}