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
    public abstract class FsClientBase
    {
        protected FsClientOptions options;

        public FsClientOptions Options 
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
        public abstract void CreateDirectory(string directory);

        public abstract void BeginUpload(string targetFilePath, int bufferSize);
        public abstract void Upload(byte[] buffer, int offset, int length);
        public abstract void EndUpload();
    }
}
