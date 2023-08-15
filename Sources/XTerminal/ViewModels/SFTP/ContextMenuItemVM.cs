using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    public class ContextMenuItemVM : ViewModelBase
    {
        private Action<FileSystemTreeVM> localExecute;
        private Action<FileSystemTreeVM> sftpExecute;

        /// <summary>
        /// 该菜单项所执行的命令
        /// </summary>
        public DelegateCommand Command { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">菜单名字</param>
        /// <param name="execute">点击本地文件树形列表和Sftp树形列表的时候要执行的动作</param>
        public ContextMenuItemVM(string name, Action<FileSystemTreeVM> execute)
        {
            this.Name = name;
            this.localExecute = execute;
            this.sftpExecute = execute;
            this.Command = new DelegateCommand(this.DelegateCommandHandler, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">菜单名字</param>
        /// <param name="localExecute">点击本地文件树形列表的时候要执行的动作</param>
        /// <param name="sftpExecute">点击Sftp树形列表的时候要执行的动作</param>
        public ContextMenuItemVM(string name, Action<FileSystemTreeVM> localExecute, Action<FileSystemTreeVM> sftpExecute)
        {
            this.Name = name;
            this.localExecute = localExecute;
            this.sftpExecute = sftpExecute;
            this.Command = new DelegateCommand(this.DelegateCommandHandler, null);
        }

        private void DelegateCommandHandler(object parameter)
        {
            FileSystemTreeVM fileSystemTree = parameter as FileSystemTreeVM;
            if (fileSystemTree is SftpFileSystemTreeVM)
            {
                this.sftpExecute(fileSystemTree);
            }
            else if (fileSystemTree is LocalFileSystemTreeVM)
            {
                this.localExecute(fileSystemTree);
            }
        }
    }
}