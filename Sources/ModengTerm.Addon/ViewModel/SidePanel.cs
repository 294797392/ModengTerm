using log4net.Repository.Hierarchy;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.Addon.ViewModel
{
    /// <summary>
    /// 所有SidePanel的基类
    /// </summary>
    public abstract class SidePanel : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PanelContentVM");

        #endregion

        #region 实例变量

        private bool isSelected;
        private string iconURI;

        #endregion

        #region 属性

        public PanelDefinition Definition { get; set; }

        public FrameworkElement Content { get; set; }

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

        #endregion

        #region 抽象方法

        public abstract void OnInitialize();

        /// <summary>
        /// 在显示之后触发
        /// </summary>
        public abstract void OnLoaded();

        /// <summary>
        /// 在从界面移出之前触发
        /// </summary>
        public abstract void OnUnload();

        public abstract void OnRelease();

        #endregion

        #region 实例方法

        #endregion

        #region 抽象方法

        #endregion
    }

    public abstract class WindowPanel : IHostPanel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WindowPanel");

        #endregion

        private int order;

        private bool isLoaded;
        private bool readyOnce;

        #region Panel

        static int id = 0;

        public override void OnInitialize()
        {
            order = ++id;
        }

        public override void OnLoaded()
        {
            logger.InfoFormat("{0} OnLoaded", order);

            isLoaded = true;
        }

        public override void OnUnload()
        {
            logger.InfoFormat("{0} OnUnload", order);

            isLoaded = false;
        }

        public override void OnRelease()
        {
        }

        #endregion
    }
}
