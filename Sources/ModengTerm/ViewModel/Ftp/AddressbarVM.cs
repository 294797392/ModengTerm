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
        public BindableCollection<DirectoryVM> DirectroyParts { get; private set; }

        public BindableCollection<DirectoryVM> PreviewDirectories { get; private set; }

        public AddressbarVM()
        {
            this.DirectroyParts = new BindableCollection<DirectoryVM>();
            this.PreviewDirectories = new BindableCollection<DirectoryVM>();
        }
    }
}