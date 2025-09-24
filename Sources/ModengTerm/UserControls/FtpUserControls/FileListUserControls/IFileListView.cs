using ModengTerm.ViewModel.Ftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModengTerm.UserControls.FtpUserControls.FileListUserControls
{
    /// <summary>
    /// 定义文件列表页面的公开接口
    /// </summary>
    public interface IFileListView
    {
        /// <summary>
        /// 获取每个项的Style
        /// </summary>
        Style ItemContainerStyle { get; }

        FtpSessionVM FtpSession { get; set; }

        /// <summary>
        /// fileItem进入编辑模式
        /// </summary>
        /// <param name="fileItem"></param>
        void BeginRename(FileItemVM fileItem);

        /// <summary>
        /// 退出编辑模式
        /// </summary>
        /// <param name="fileItem"></param>
        void EndRename(FileItemVM fileItem);
    }
}