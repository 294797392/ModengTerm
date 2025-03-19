using ModengTerm.Base.Definitions;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class PanelItemVM : MenuItemVM
    {
        //private string svgPath;

        //public string SVGPath
        //{
        //    get { return this.svgPath; }
        //    set
        //    {
        //        if (this.svgPath != value)
        //        {
        //            this.svgPath = value;
        //            this.NotifyPropertyChanged("SVGPath");
        //        }
        //    }
        //}
    }

    public class PanelVM : AbstractMenuVM<PanelItemVM>
    {
        #region 实例变量

        private bool visible;
        private PanelAlignEnum dock;

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

        public PanelAlignEnum Dock
        {
            get { return this.dock; }
            set
            {
                if (this.dock != value)
                {
                    this.dock = value;
                    this.NotifyPropertyChanged("Dock");
                }
            }
        }

        #endregion

        #region 构造方法

        public PanelVM()
        {
        }

        #endregion

        #region 实例方法

        private void ProcessLoaded(bool loaded, PanelItemVM panelItemVM)
        {
            if (panelItemVM.ContentVM == null)
            {
                return;
            }

            if (!(panelItemVM.ContentVM is MenuContentVM))
            {
                return;
            }

            MenuContentVM menuContentVM = panelItemVM.ContentVM as MenuContentVM;

            if (loaded)
            {
                menuContentVM.OnLoaded();
            }
            else
            {
                menuContentVM.OnUnload();
            }
        }

        #endregion

        #region 公开接口

        public void ChangeVisible(string panelItemId)
        {
            PanelItemVM panelItemVM = MenuItems.FirstOrDefault(v => v.ID.ToString() == panelItemId);

            // 当前状态
            bool visible = false;

            if (Visible)
            {
                if (SelectedMenu == panelItemVM)
                {
                    visible = true;
                }
            }

            if (visible)
            {
                // 当前是显示状态，隐藏
                Visible = false;
                ProcessLoaded(false, panelItemVM);
            }
            else
            {
                // 当前是隐藏状态，显示
                Visible = true;

                if (!panelItemVM.IsSelected)
                {
                    // 选中之后会自动触发Loaded事件
                    panelItemVM.IsSelected = true;
                }
                else
                {
                    ProcessLoaded(true, panelItemVM);
                }
            }
        }

        #endregion
    }
}
