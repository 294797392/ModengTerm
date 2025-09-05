using ModengTerm.FileTrans.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.FileTrans
{
    public class FsTreeContext : TreeViewModelContext
    {
        /// <summary>
        /// 获取是服务器树形列表还是客户端树形列表
        /// </summary>
        public FsTreeTypeEnum Type { get; set; }
    }

    public class FsTreeVM : TreeViewModel<FsTreeContext>
    {
        private string currentDirectory;

        public string CurrentDirectory
        {
            get { return this.currentDirectory; }
            set
            {
                if (this.currentDirectory != value)
                {
                    this.currentDirectory = value;
                    this.NotifyPropertyChanged("CurrentDirectory");
                }
            }
        }
    }
}
