using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using System;
using System.Linq;
using System.Windows.Automation;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panel
{
    public class PanelContainer : ViewModelBase
    {
        #region 实例变量

        private bool visible;
        private HostSidePanel selectedItem;

        #endregion

        #region 属性

        public BindableCollection<HostSidePanel> Panels { get; private set; }

        public HostSidePanel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    NotifyPropertyChanged("SelectedItem");
                }
            }
        }

        public SidePanelDocks Dock { get; set; }

        #endregion

        #region 构造方法

        public PanelContainer()
        {
            Panels = new BindableCollection<HostSidePanel>();
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        public void VisiblePanel(string panelId)
        {
            HostSidePanel panel = Panels.FirstOrDefault(v => v.ID == panelId);
            if (panel == null)
            {
                return;
            }

            // 当前状态
            bool visible = false;

            if (panel.IsSelected)
            {
                visible = true;
            }

            if (visible)
            {
                // 当前是显示状态，隐藏
                panel.IsSelected = false;
            }
            else
            {
                // 当前是隐藏状态，显示
                panel.IsSelected = true;
            }
        }

        #endregion
    }
}
