using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.ViewModels.SFTP
{
    public class LocalFileSystemTreeVM : FileSystemTreeVM
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("LocalFileSystemTreeVM");

        public override void AppendSubDirectory(FileSystemTreeNodeVM parentDirectory)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<FileSystemTreeNodeVM> GetDirectory(string directory)
        {
            IEnumerable<string> subPaths = null;

            try
            {
                subPaths = Directory.EnumerateFileSystemEntries(directory, "*", SearchOption.TopDirectoryOnly);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.Error("没有权限访问目录, {0}", ex);
                yield break;
            }
            catch (Exception ex)
            {
                logger.Error("枚举子目录异常", ex);
                yield break;
            }

            foreach (string path in subPaths)
            {
                FileSystemTreeNodeVM fsNode = null;

                FileInfo fileInfo = new FileInfo(path);

                FileAttributes fileAttributes = fileInfo.Attributes;

                if (fileAttributes.HasFlag(FileAttributes.Directory))
                {
                    fsNode = new DirectoryNodeVM(this.Context);
                }
                else
                {
                    fsNode = new FileNodeVM(this.Context);
                    fsNode.Size = fileInfo.Length;
                }

                fsNode.ID = path;
                fsNode.Name = fileInfo.Name;
                fsNode.IsHidden = fileAttributes.HasFlag(FileAttributes.Hidden);
                fsNode.FullPath = fileInfo.FullName;
                fsNode.LastUpdateTime = fileInfo.LastWriteTime;

                yield return fsNode;
            }
        }
    }
}
