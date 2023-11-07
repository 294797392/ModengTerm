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
        private string filePath;

        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                    this.NotifyPropertyChanged("FilePath");
                }
            }
        }

        public RecordOptionsVM() 
        {
        }
    }
}
