using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.ViewModels.SFTP
{
    public class SftpFileSystemTreeVM : FileSystemTreeVM
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("SftpFileSystemTreeVM");

        public SftpFileSystemTreeVM()
        {
        }

        public override void AppendSubDirectory(FileSystemTreeNodeVM parentDirectory)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FileSystemTreeNodeVM> GetDirectory(string directory)
        {
            IEnumerable<SftpFile> fileList = null;

            try
            {
                fileList = this.SftpClient.ListDirectory(directory);
            }
            catch (Exception ex)
            {
                logger.Error("枚举子目录异常", ex);
                yield break;
            }

            foreach (SftpFile file in fileList)
            {
                FileSystemTreeNodeVM fsNode = null;

                if (file.IsDirectory)
                {
                    fsNode = new DirectoryNodeVM(this.Context);
                }
                else
                {
                    fsNode = new FileNodeVM(this.Context);
                    fsNode.Size = file.Length;
                }

                fsNode.ID = file.FullName;
                fsNode.Name = file.Name;
                fsNode.IsHidden = file.Name.StartsWith(".");
                fsNode.FullPath = file.FullName;
                fsNode.LastUpdateTime = file.LastWriteTime;

                yield return fsNode;
            }
        }
    }
}

