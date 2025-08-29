using DotNEToolkit;
using ModengTerm.Addon;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using WPFToolkit.MVVM;

namespace ModengTerm.Base
{
    /// <summary>
    /// 存储整个应用程序都需要使用的通用的数据和方法
    /// </summary>
    public class ClientContext : ModularApp<ClientContext, ClientManifest>, INotifyPropertyChanged
    {
        #region 实例变量

        #endregion

        #region 属性

        #endregion

        #region ModularApp

        protected override int OnInitialized()
        {
            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
