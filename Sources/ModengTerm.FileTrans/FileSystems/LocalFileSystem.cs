using ModengTerm.Base;
using ModengTerm.FileTrans.DataModels;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.Ftp.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    public class LocalFileSystem : FileSystem
    {
        #region WinAPI

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        const int FO_MOVE = 1;
        const short FOF_NOERRORUI = 0x400;         // 不显示错误对话框
        const int FO_DELETE = 3;
        const short FOF_ALLOWUNDO = 0x40;
        const short FOF_NOCONFIRMATION = 0x10;

        private static bool DeleteToRecycleBinLegacy(string path)
        {
            SHFILEOPSTRUCT fileOp = new SHFILEOPSTRUCT
            {
                wFunc = FO_DELETE,
                pFrom = path + '\0', // 必须以 \0 结尾
                fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION
            };

            return SHFileOperation(ref fileOp) == 0;
        }

        public static int RenameFileWithShell(string oldPath, string newPath, bool allowUndo = false)
        {
            SHFILEOPSTRUCT fileOp = new SHFILEOPSTRUCT();
            fileOp.wFunc = FO_MOVE;
            fileOp.pFrom = oldPath + '\0';  // 必须双 null 结尾
            fileOp.pTo = newPath + '\0';    // 必须双 null 结尾

            short flags = FOF_NOCONFIRMATION | FOF_NOERRORUI;
            if (allowUndo) flags |= FOF_ALLOWUNDO;
            fileOp.fFlags = flags;

            return SHFileOperation(ref fileOp);
        }


        #endregion

        private static log4net.ILog logger = log4net.LogManager.GetLogger("LocalFsClient");

        #region 实例变量

        private string currentDirectory;

        #endregion

        #region 实例方法

        #endregion

        public override int Open()
        {
            return ResponseCode.SUCCESS;
        }

        public override void Close()
        {
        }

        public override List<FsItemInfo> ListItems(string directory)
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
            Directory.CreateDirectory(directory);
        }

        public override void DeleteFile(string filePath)
        {
            DeleteToRecycleBinLegacy(filePath);

            //File.Delete(filePath);
        }

        public override void DeleteDirectory(string directoryPath)
        {
            DeleteToRecycleBinLegacy(directoryPath);
        }

        public override void RenameFile(string oldPath, string newPath)
        {
            File.Move(oldPath, newPath);
        }

        public override bool IsFileEixst(string filePath)
        {
            return File.Exists(filePath);
        }

        public override void RenameDirectory(string oldPath, string newPath)
        {
            Directory.Move(oldPath, newPath);
        }

        public override bool IsDirectoryExist(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }




        public override Stream OpenRead(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public override Stream OpenWrite(string filePath)
        {
            return new FileStream(filePath, FileMode.Create, FileAccess.Write);
        }
    }
}
