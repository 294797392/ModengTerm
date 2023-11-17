using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    /// <summary>
    /// 录制选项窗口的ViewModel
    /// </summary>
    public class RecordOptionsVM : ViewModelBase
    {
        private string fileName;

        public string FileName
        {
            get { return this.fileName; }
            set
            {
                if (this.fileName != value)
                {
                    this.fileName = value;
                    this.NotifyPropertyChanged("FileName");
                }
            }
        }

        public RecordOptionsVM() 
        {
        }

        public bool ParameterVerfication()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                MTMessageBox.Info("请输入文件名");
                return false;
            }

            return true;
        }
    }
}
