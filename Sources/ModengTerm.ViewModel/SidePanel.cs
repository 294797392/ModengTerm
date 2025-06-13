using ModengTerm.Addon.Extensions;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel
{
    /// <summary>
    /// 提供扩展侧边栏的接口
    /// </summary>
    public class SidePanel : ViewModelBase, IHostSidePanel
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

        /// <summary>
        /// 界面
        /// </summary>
        public FrameworkElement Content { get; set; }

        /// <summary>
        /// 扩展的对象
        /// </summary>
        public SidePanelExtension ExtensionObject { get; set; }

        public bool IsOpened
        {
            get { return this.IsSelected; }
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.ExtensionObject.OnInitialize();
        }

        /// <summary>
        /// 在显示之后触发
        /// </summary>
        public void Loaded()
        {
            this.ExtensionObject.OnLoaded();
        }

        /// <summary>
        /// 在从界面移出之前触发
        /// </summary>
        public void Unloaded()
        {
            this.ExtensionObject.OnUnload();
        }

        public void Release()
        {
            this.ExtensionObject.OnRelease();
        }

        /// <summary>
        /// 打开侧边栏
        /// </summary>
        public void Open()
        {
            this.IsSelected = true;
        }

        /// <summary>
        /// 关闭侧边栏
        /// </summary>
        public void Close()
        {
            this.IsSelected = false;
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
    }
}
