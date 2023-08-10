using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base.DataModels;
using XTerminal.Session;
using XTerminal.UserControls;

namespace XTerminal.ViewModels
{
    public abstract class OpenedSessionVM : ItemViewModel
    {
        #region 公开事件

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        public event Action<OpenedSessionVM, SessionStatusEnum> StatusChanged;

        #endregion

        #region 属性

        /// <summary>
        /// 界面上的控件
        /// </summary>
        public SessionContent Content { get; set; }

        #endregion

        public abstract int Open(XTermSession session);

        public abstract void Close();
    }
}
