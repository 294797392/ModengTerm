using ModengTerm.FileTrans.DataModels;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients
{
    /// <summary>
    /// 文件系统管理客户端
    /// 提供文件的读取，删除，上传，下载功能
    /// </summary>
    public abstract class FileSystem
    {
        protected FileSystemOptions options;

        public FileSystemOptions Options
        {
            get { return this.options; }
            set
            {
                if (this.options != value)
                {
                    this.options = value;
                }
            }
        }

        /// <summary>
        /// 打开到服务器的连接
        /// </summary>
        /// <returns></returns>
        public abstract int Open();

        /// <summary>
        /// 关闭到服务器的连接
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 列出当前目录里的所有目录和文件
        /// </summary>
        /// <returns></returns>
        public abstract List<FsItemInfo> ListFiles(string directory);

        /// <summary>
        /// 改变当前目录
        /// </summary>
        /// <param name="directory">要改变到的新目录</param>
        public abstract void ChangeDirectory(string directory);

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <remarks>操作失败直接扔异常，不扔异常就说明操作成功</remarks>
        public abstract void CreateDirectory(string directoryPath);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">要删除的文件完整路径</param>
        public abstract void DeleteFile(string filePath);

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns></returns>
        public abstract bool IsFileEixst(string filePath);

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="directoryPath">要删除的目录完整路径</param>
        public abstract void DeleteDirectory(string directoryPath);

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public abstract void RenameFile(string oldPath, string newPath);

        /// <summary>
        /// 重命名目录
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public abstract void RenameDirectory(string oldPath, string newPath);

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录完整路径</param>
        /// <returns></returns>
        public abstract bool IsDirectoryExist(string directoryPath);




        #region 读写接口

        /// <summary>
        /// 打开一个文件流用于读取
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public abstract Stream OpenRead(string filePath);

        /// <summary>
        /// 打开一个文件流用于写入
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public abstract Stream OpenWrite(string filePath);

        #endregion
    }
}
