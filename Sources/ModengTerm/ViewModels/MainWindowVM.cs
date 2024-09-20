using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 控制主界面视图的显示/隐藏
    /// </summary>
    public class ViewControlVM : ViewModelBase
    {
        private bool shellCommandView;

        public bool ShellCommandView
        {
            get { return shellCommandView; }
            set
            {
                if (this.shellCommandView != value)
                {
                    this.shellCommandView = value;
                    this.NotifyPropertyChanged("ShellCommandView");
                }
            }
        }
    }

    public class MainWindowVM : ViewModelBase
    {
        public ViewControlVM ViewControl { get; private set; }

        public MainWindowVM()
        {
            this.ViewControl = new ViewControlVM();
        }
    }
}


