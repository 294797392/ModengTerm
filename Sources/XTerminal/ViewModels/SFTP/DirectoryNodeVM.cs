using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    public class DirectoryNodeVM : FileSystemTreeNodeVM
    {
        public override FileSystemNodeTypeEnum Type => FileSystemNodeTypeEnum.Directory;

        public DirectoryNodeVM(TreeViewModelContext context, object data = null) :
            base(context, data)
        { }
    }
}
