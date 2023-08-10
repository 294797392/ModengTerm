using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    public class SftpTreeVM : TreeViewModel<SftpTreeViewModelContext>
    {
        private string initialDirectory;

        /// <summary>
        /// 初始目录
        /// </summary>
        public string InitialDirectory 
        {
            get { return this.initialDirectory; }
            set
            {
                if (this.initialDirectory != value)
                {
                    this.initialDirectory = value;
                    this.NotifyPropertyChanged("InitialDirectory");
                }
            }
        }
    }
}
