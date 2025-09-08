using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Clients.Channels
{
    /// <summary>
    /// 封装文件上传，下载通道
    /// 每个通道独立进行上传，下载，互不干扰
    /// </summary>
    public interface FsChannel
    {
        #region 抽象方法

        /// <summary>
        /// 打开上传通道
        /// </summary>
        /// <param name="path">要打开的文件路径</param>
        /// <returns></returns>
        void OpenWrite(string path);

        /// <summary>
        /// 打开下载通道
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        void OpenRead(string path);

        /// <summary>
        /// 关闭文件通道
        /// </summary>
        void Close();

        #endregion

        void Write(byte[] bytes, int offset, int size);
        void Read(byte[] buffer, int offset, int size);

        bool CreateDirectory(string dir);
    }
}