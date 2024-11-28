using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public enum ToolPanelTypeEnum
    {
        /// <summary>
        /// 资源管理器
        /// </summary>
        ResourceManager,

        /// <summary>
        /// 快捷命令输入窗格
        /// </summary>
        QuickCommand,

        /// <summary>
        /// 进程管理
        /// </summary>
        ProcessManager,

        /// <summary>
        /// 系统监控
        /// </summary>
        SystemWatch
    }

    public class ToolPanelItemVM : MenuItemVM
    {
        public ToolPanelTypeEnum Type { get; set; }

        /// <summary>
        /// 该PanelItem属于哪个Panel
        /// </summary>
        public ToolPanelVM OwnerPanel { get; set; }

        public ToolPanelItemVM() { }

        public ToolPanelItemVM(MenuDefinition menu) :
            base(menu)
        {
        }
    }

    public class ToolPanelVM : AbstractMenuVM<ToolPanelItemVM>
    {
        private bool visible;

        /// <summary>
        /// 是否显示该菜单
        /// </summary>
        public bool Visible
        {
            get { return this.visible; }
            set
            {
                if (this.visible != value)
                {
                    this.visible = value;
                    this.NotifyPropertyChanged("Visible");
                }
            }
        }

        public ToolPanelVM()
        {
        }
    }
}
