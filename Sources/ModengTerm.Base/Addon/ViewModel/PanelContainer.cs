using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using System;
using System.Linq;
using System.Windows.Automation;
using WPFToolkit.MVVM;

namespace ModengTerm.Base.Addon.ViewModel
{
    public class PanelContainer : ViewModelBase
    {
        #region 实例变量

        private bool visible;
        private PanelBase selectedItem;

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

        public BindableCollection<PanelBase> Panels { get; private set; }

        public PanelBase SelectedItem
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
            this.Panels = new BindableCollection<PanelBase>();
        }

        #endregion

        #region 实例方法

        private void ProcessLoaded(bool loaded, PanelBase panel)
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

        public void ChangeVisible(string panelId)
        {
            PanelBase panel = this.Panels.FirstOrDefault(v => v.ID.ToString() == panelId) as PanelBase;
            if (panel == null)
            {
                return;
            }

            // 当前状态
            bool visible = false;

            if (this.Visible)
            {
                if (panel.IsSelected)
                {
                    visible = true;
                }
            }

            if (visible)
            {
                // 当前是显示状态，隐藏
                this.Visible = false;
                this.Panels.SelectedItem.IsSelected = false;
                ProcessLoaded(false, this.Panels.SelectedItem);
            }
            else
            {
                // 当前是隐藏状态，显示
                Visible = true;

                if (!panel.IsSelected)
                {
                    // 选中之后会自动触发Loaded事件
                    panel.IsSelected = true;
                }
                else
                {
                    ProcessLoaded(true, panel);
                }
            }
        }

        #endregion
    }
}
