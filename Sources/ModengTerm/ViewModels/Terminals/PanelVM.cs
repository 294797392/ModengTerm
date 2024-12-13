using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    public class PanelItemVM : MenuItemVM
    {
        public PanelVM OwnerPanel { get; set; }
    }

    public class PanelVM : AbstractMenuVM<PanelItemVM>
    {
        /// <summary>
        /// 用户手动关闭的时候触发
        /// </summary>
        public Action<PanelVM> CloseDelegate { get; set; }

        /// <summary>
        /// 用户手动切换内容的时候触发
        /// </summary>
        public Action<PanelVM, PanelItemVM, PanelItemVM> SelectionChangedDelegate { get; set; }

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
