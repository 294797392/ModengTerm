using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using WPFToolkit.MVVM;
using log4net.Repository.Hierarchy;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base;
using ModengTerm.Terminal;
using System.Windows.Threading;
using System.Windows.Media;
using System.Reflection.Metadata;

namespace ModengTerm.ViewModel
{
    /// <summary>
    /// 打开的所有会话列表ViewModel
    /// </summary>
    public class OpenedSessionsVM : ViewModelBase
    {
        #region 类变量


        #endregion

        #region 公开事件

        /// <summary>
        /// 当会话被完全打开之后触发
        /// </summary>
        public event Action<OpenedSessionsVM, OpenedSessionVM> OnSessionOpened;

        #endregion

        #region 公开属性

        #endregion

        #region 构造方法

        public OpenedSessionsVM()
        {
        }

        #endregion

        #region 公开接口


        #endregion
    }
}
