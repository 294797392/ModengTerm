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
    public class ParentDirectoryVM : ItemViewModel
    {
        private BitmapSource icon;
        private string fullPath;

        public BitmapSource Icon
        {
            get { return this.icon; }
            set
            {
                if (this.icon != value)
                {
                    this.icon = value;
                    this.NotifyPropertyChanged("Icon");
                }
            }
        }

        public string FullPath
        {
            get { return this.fullPath; }
            set
            {
                if (this.fullPath != value)
                {
                    this.fullPath = value;
                    this.NotifyPropertyChanged("FullPath");
                }
            }
        }

        public ParentDirectoryVM(string name, string directoryPath)
        {
            this.ID = directoryPath;
            this.Name = name;
            this.FullPath = directoryPath;
            this.Icon = IconUtils.GetFolderIcon(directoryPath);
        }

        public ParentDirectoryVM(string name, string directoryPath, BitmapSource icon)
        {
            this.ID = directoryPath;
            this.Name = name;
            this.FullPath = directoryPath;
            this.icon = icon;
        }
    }
}
