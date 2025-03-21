using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using System;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 所有PanelContent的基类
    /// TODO：尝试把PanelContentVM改造成插件
    /// </summary>
    public abstract class WindowContentVM : MenuContentVM
    {
        #region 类变量

        public static readonly string KEY_OPENED_SESSION = Guid.NewGuid().ToString();

        #endregion

        #region 实例变量

        private bool readyOnce;
        private bool isLoaded;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前会话的连接状态
        /// </summary>
        protected SessionStatusEnum SessionStatus { get { return this.OpenedSession.Status; } }

        protected OpenedSessionVM OpenedSession { get; set; }

        #endregion

        #region MenuContentVM

        public override void OnInitialize()
        {
            this.OpenedSession = this.Parameters[KEY_OPENED_SESSION] as OpenedSessionVM;
        }

        public override void OnLoaded()
        {
            this.isLoaded = true;

            this.RaiseOnReady();
        }

        public override void OnUnload()
        {
            this.isLoaded = false;
        }

        public override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        private void RaiseOnReady()
        {
            if (this.readyOnce)
            {
                return;
            }

            if (!this.isLoaded)
            {
                return;
            }

            if (this.SessionStatus != SessionStatusEnum.Connected)
            {
                return;
            }

            this.OnReady();

            this.readyOnce = true;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        /// <param name="status"></param>
        public virtual void OnStatusChanged(SessionStatusEnum status)
        {
            switch (status)
            {
                case SessionStatusEnum.Connected:
                    {
                        this.RaiseOnReady();
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>
        /// 当以下两个条件全部成立的时候触发：
        /// 1. 会话状态是连接成功
        /// 2. 页面当前是显示状态
        /// 只会触发一次
        /// </summary>
        public virtual void OnReady()
        { }

        #endregion
    }

    public class SidePanelItemVM : MenuItemVM
    {
        public SidePanelItemVM() 
        {
        }
    }

    public class SidePanelVM : AbstractMenuVM<SidePanelItemVM>
    {
        #region 实例变量

        private bool visible;
        private SideWindowDock dock;

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

        public SideWindowDock Dock
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

        public SidePanelVM()
        {
        }

        #endregion

        #region 实例方法

        private void ProcessLoaded(bool loaded, SidePanelItemVM panelItemVM)
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
            SidePanelItemVM panelItemVM = MenuItems.FirstOrDefault(v => v.ID.ToString() == panelItemId);

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
