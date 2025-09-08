using ModengTerm.Base;
using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    public class LocalFsClient : FsClientBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("LocalFsClient");

        private string currentDirectory;

        public override int Open()
        {
            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
        }

        public override List<FsItemInfo> ListFiles(string directory)
        {
            List<FsItemInfo> fsItems = new List<FsItemInfo>();

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            // 先列举目录
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dirInfo in directoryInfos)
            {
                FsItemInfo fsItem = new FsItemInfo()
                {
                    FullPath = dirInfo.FullName,
                    LastUpdateTime = dirInfo.LastWriteTime,
                    Name = dirInfo.Name,
                    Type = FsItemTypeEnum.Directory,
                    IsHidden = dirInfo.Attributes.HasFlag(FileAttributes.Hidden)
                };
                fsItems.Add(fsItem);
            }

            // 再列举文件
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                FsItemInfo fsItem = new FsItemInfo()
                {
                    Size = fileInfo.Length,
                    FullPath = fileInfo.FullName,
                    LastUpdateTime = fileInfo.LastWriteTime,
                    Name = fileInfo.Name,
                    Type = FsItemTypeEnum.File,
                    IsHidden = fileInfo.Attributes.HasFlag(FileAttributes.Hidden)
                };
                fsItems.Add(fsItem);
            }

            return fsItems;
        }

        public override void ChangeDirectory(string directory)
        {
            this.currentDirectory = directory;
        }

        public override void CreateDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public override void BeginUpload(string targetFilePath, int bufferSize)
        {
        }

        public override void Upload(byte[] buffer, int offset, int length)
        { }

        public override void EndUpload()
        { }
    }
}
