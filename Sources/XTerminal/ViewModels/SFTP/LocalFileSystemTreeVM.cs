using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.ViewModels.SFTP
{
    public class LocalFileSystemTreeVM : FileSystemTreeVM
    {
        public override void AppendSubDirectory(FileSystemTreeNodeVM parentDirectory)
        {
            throw new NotImplementedException();
        }

        public override void LoadSubDirectory(string directory)
        {
            throw new NotImplementedException();
        }
    }
}
