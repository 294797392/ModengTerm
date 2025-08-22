using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Addon;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panels
{
    public class PanelContainer : ViewModelBase
    {
        #region 实例变量

        private bool visible;
        private SidePanelVM selectedItem;

        #endregion

        #region 属性

        public BindableCollection<SidePanelVM> Panels { get; private set; }

        public SidePanelVM SelectedItem
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
            Panels = new BindableCollection<SidePanelVM>();
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        public void VisiblePanel(string panelId)
        {
            SidePanelVM panel = Panels.FirstOrDefault(v => v.ID == panelId);
            if (panel == null)
            {
                return;
            }

            // 当前状态
            bool visible = false;

            if (panel.IsOpened)
            {
                visible = true;
            }

            if (visible)
            {
                // 当前是显示状态，隐藏
                panel.IsOpened = false;
            }
            else
            {
                // 当前是隐藏状态，显示
                panel.IsOpened = true;
            }
        }

        #endregion
    }
}
