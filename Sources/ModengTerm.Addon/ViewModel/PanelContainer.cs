using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using System;
using System.Linq;
using System.Windows.Automation;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.ViewModel
{
    public class PanelContainer : ViewModelBase
    {
        #region 实例变量

        private bool visible;
        private Panel selectedItem;

        #endregion

        #region 属性

        public BindableCollection<Panel> Panels { get; private set; }

        public Panel SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (this.selectedItem != value)
                {
                    this.selectedItem = value;
                    this.NotifyPropertyChanged("SelectedItem");
                }
            }
        }

        #endregion

        #region 构造方法

        public PanelContainer()
        {
            this.Panels = new BindableCollection<Panel>();
        }

        #endregion

        #region 实例方法

        private void ProcessLoaded(bool loaded, Panel panel)
        {
            if (panel.Content == null)
            {
                return;
            }

            if (loaded)
            {
                panel.OnLoaded();
            }
            else
            {
                panel.OnUnload();
            }
        }

        #endregion

        #region 公开接口

        public void VisiblePanel(string panelId)
        {
            Panel panel = this.Panels.FirstOrDefault(v => v.ID.ToString() == panelId);
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
