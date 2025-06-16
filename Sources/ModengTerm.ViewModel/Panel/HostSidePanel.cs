using ModengTerm.Addon.Interactive;
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
    /// <summary>
    /// 提供扩展侧边栏的接口
    /// </summary>
    public class HostSidePanel : HostPanel
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

        public override bool IsOpened => this.IsSelected;

        #endregion

        #region HostPanelBase

        /// <summary>
        /// 打开侧边栏
        /// </summary>
        public override void Open()
        {
            this.IsSelected = true;
        }

        /// <summary>
        /// 关闭侧边栏
        /// </summary>
        public override void Close()
        {
            this.IsSelected = false;
        }

        #endregion
    }
}
