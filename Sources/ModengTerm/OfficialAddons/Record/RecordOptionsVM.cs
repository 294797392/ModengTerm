using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Record
{
    /// <summary>
    /// 录制选项窗口的ViewModel
    /// </summary>
    public class RecordOptionsVM : ViewModelBase
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    NotifyPropertyChanged("FilePath");
                }
            }
        }

        public RecordOptionsVM()
        {
        }
    }
}
