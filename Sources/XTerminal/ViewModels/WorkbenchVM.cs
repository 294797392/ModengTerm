using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace XTerminal.ViewModels
{
    public class WorkbenchVM : ViewModelBase
    {
        /// <summary>
        /// 打开了的所有会话信息
        /// </summary>
        public BindableCollection<OpenedSessionVM> OpenedSessions { get; private set; }

        public WorkbenchVM()
        {
            this.OpenedSessions = new BindableCollection<OpenedSessionVM>();
        }
    }
}
