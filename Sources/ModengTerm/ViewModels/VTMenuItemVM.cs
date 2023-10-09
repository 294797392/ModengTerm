using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class VTMenuItemVM : ViewModelBase
    {
        /// <summary>
        /// 该菜单所对应的终端
        /// </summary>
        public IVideoTerminal VideoTerminal { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public BindableCollection<VTMenuItemVM> Children { get; set; }

        public VTMenuItemVM(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        public void Handle()
        { }
    }
}
