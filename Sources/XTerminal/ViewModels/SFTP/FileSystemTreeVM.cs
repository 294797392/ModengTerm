using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels.SFTP
{
    public abstract class FileSystemTreeVM : TreeViewModel<FileSystemTreeVMContext>
    {
        #region 实例变量

        private string initialDirectory;

        #endregion

        #region 属性

        /// <summary>
        /// 初始目录
        /// </summary>
        public string InitialDirectory 
        {
            get { return this.initialDirectory; }
            set
            {
                if (this.initialDirectory != value)
                {
                    this.initialDirectory = value;
                    this.NotifyPropertyChanged("InitialDirectory");
                }
            }
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.LoadSubDirectory(this.InitialDirectory);
        }

        public void Release()
        { }

        #endregion

        #region 事件处理器

        public void MouseDoubleClickEventHandler()
        {
            FileSystemTreeNodeVM selectedNode = this.Context.SelectedItem as FileSystemTreeNodeVM;
            if (selectedNode == null)
            {
                return;    
            }

            // 重新加载文件系统树形列表
        }

        #endregion

        #region 抽象方法

        public abstract void AppendSubDirectory(FileSystemTreeNodeVM parentDirectory);

        public abstract void LoadSubDirectory(string directory);

        #endregion
    }
}
