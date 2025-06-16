using ModengTerm.Addon.Client;
using ModengTerm.Addon.Interactive;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panel
{
    public abstract class HostPanel : ViewModelBase
    {
        #region 属性

        public IPanelBase ClientPanel { get; set; }

        /// <summary>
        /// 界面
        /// </summary>
        public FrameworkElement Content { get; set; }

        public abstract bool IsOpened { get; }

        #endregion

        #region 公开接口

        public void Initialize(HostContext context)
        {
            this.ClientPanel.OnInitialize(context);
        }

        /// <summary>
        /// 在显示之后触发
        /// </summary>
        public void Loaded()
        {
            this.ClientPanel.OnLoaded();
        }

        /// <summary>
        /// 在从界面移出之前触发
        /// </summary>
        public void Unloaded()
        {
            this.ClientPanel.OnUnload();
        }

        public void Release()
        {
            this.ClientPanel.OnRelease();
        }

        public void SwitchStatus()
        {
            if (this.IsOpened)
            {
                this.Close();
            }
            else
            {
                this.Open();
            }
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 打开面板
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// 关闭面板
        /// </summary>
        public abstract void Close();

        #endregion
    }
}
