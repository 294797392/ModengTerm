using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    public enum PanelContentTypeEnum
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

    public class PanelVM : AbstractMenuVM<ContextMenuVM>
    {
        #region 实例变量

        private bool visible;

        #endregion

        #region 属性

        /// <summary>
        /// 是否显示该窗口
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    NotifyPropertyChanged("Visible");
                }
            }
        }

        #endregion

        #region 构造方法

        public PanelVM()
        {
        }

        #endregion
    }
}
