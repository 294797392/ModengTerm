using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.ViewModels.SFTP
{
    public class SftpFileSystemTreeVM : FileSystemTreeVM
    {
        private SftpClient sftpClient;

        public SftpFileSystemTreeVM(SftpClient sftpClient)
        {
            this.sftpClient = sftpClient;
        }

        public override void AppendSubDirectory(FileSystemTreeNodeVM parentDirectory)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FileSystemTreeNodeVM> GetDirectory(string directory)
        {
            throw new NotImplementedException();
        }
    }
}
