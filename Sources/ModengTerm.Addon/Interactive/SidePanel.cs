using log4net.Repository.Hierarchy;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 指定SidePanel要显示的位置
    /// </summary>
    public enum SidePanelDocks
    {
        /// <summary>
        /// 显示在左边
        /// </summary>
        Left,

        /// <summary>
        /// 显示在右边
        /// </summary>
        Right
    }

    /// <summary>
    /// 提供扩展侧边栏的接口
    /// </summary>
    public abstract class SidePanel : ViewModelBase
    {
        #region 实例变量

        private bool isSelected;
        private string iconURI;

        #endregion

        #region 属性

        public SidePanelDocks Dock { get; set; }

        public PanelDefinition Definition { get; set; }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public string IconURI
        {
            get { return this.iconURI; }
            set
            {
                if (this.iconURI != value)
                {
                    this.iconURI = value;
                    this.NotifyPropertyChanged("IconURI");
                }
            }
        }

        public FrameworkElement Content { get; set; }

        #endregion

        #region 公开接口

        public void Initialize() 
        {
            this.OnInitialize();
        }

        /// <summary>
        /// 在显示之后触发
        /// </summary>
        public void Loaded() 
        {
            this.OnUnload();
        }

        /// <summary>
        /// 在从界面移出之前触发
        /// </summary>
        public void Unloaded() 
        {
            this.OnUnload();
        }

        public void Release() 
        {
            this.OnRelease();
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();

        protected abstract void OnLoaded();

        protected abstract void OnUnload();

        protected abstract void OnRelease();

        #endregion
    }
}
