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
        private string fullPath;

        public string FullPath
        {
            get { return fullPath; }
            set
            {
                if (fullPath != value)
                {
                    fullPath = value;
                    NotifyPropertyChanged("FullPath");
                }
            }
        }

        public RecordOptionsVM()
        {
        }
    }
}
