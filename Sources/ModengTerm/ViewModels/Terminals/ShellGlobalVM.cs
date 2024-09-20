using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    /// <summary>
    /// 维护Shell类型的会话里的全局数据
    /// </summary>
    public class ShellGlobalVM : ViewModelBase
    {
        /// <summary>
        /// 快捷命令列表
        /// </summary>
        public BindableCollection<ShellCommandVM> Commands { get; private set; }

        public ShellGlobalVM()
        {
            this.Commands = new BindableCollection<ShellCommandVM>();
        }
    }
}
