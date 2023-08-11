using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    public abstract class FileSystemTreeNodeVM : TreeNodeViewModel
    {
        /// <summary>
        /// 当前文件/目录的完整路径
        /// </summary>
        public string FullPath { get; set; }

        public FileSystemTreeNodeVM(TreeViewModelContext context, object data = null) : 
            base(context, data)
        {
        }
    }
}
