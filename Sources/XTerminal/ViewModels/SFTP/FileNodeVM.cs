using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    public class FileNodeVM : FileSystemTreeNodeVM
    {
        public override FileSystemNodeTypeEnum Type => FileSystemNodeTypeEnum.File;

        public FileNodeVM(TreeViewModelContext context, object data = null) :
            base(context, data)
        { }
    }
}
