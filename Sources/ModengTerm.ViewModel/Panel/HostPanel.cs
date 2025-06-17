using ModengTerm.Addon.Client;
using ModengTerm.Addon.Panel;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Panel
{
    public abstract class HostPanel : ViewModelBase
    {
        #region 实例变量

        private bool isOpened;
        private string iconURI;
        private FrameworkElement content;

        #endregion

        #region 公开属性

        public PanelDefinition Definition { get; set; }

        public IAddonPanel ClientPanel { get { return this.Content as IAddonPanel; } }

        public FrameworkElement Content 
        {
            get { return this.content; }
            set
            {
                if (this.content != value) 
                {
                    this.content = value;
                    this.NotifyPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// 该面板是否打开
        /// </summary>
        public bool IsOpened
        {
            get { return this.isOpened; }
            set
            {
                if (this.isOpened != value)
                {
                    this.isOpened = value;
                    this.NotifyPropertyChanged("IsOpened");
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

        #region 公开接口

        /// <summary>
        /// 打开侧边栏
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// 关闭侧边栏
        /// </summary>
        public abstract void Close();

        public void Initialize(PanelContext context)
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
            if (this.isOpened)
            {
                this.Close();
            }
            else
            {
                this.Open();
            }
        }

        #endregion
    }
}
