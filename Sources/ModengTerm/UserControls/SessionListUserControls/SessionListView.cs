using ModengTerm.Base.DataModels;
using ModengTerm.ViewModels.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModengTerm.UserControls.SessionListUserControls
{
    public abstract class SessionListView : UserControl
    {
        public event Action<XTermSessionVM> OpenSessionEvent;

        public SessionTreeVM SessionTreeVM { get; set; }

        public SessionGroupVM CurrentGroup { get; set; }

        public abstract void OnLoad();

        public abstract void OnUnload();

        protected void NotifyOpenSessionEvent(XTermSessionVM sessionVM)
        {
            if (this.OpenSessionEvent != null)
            {
                this.OpenSessionEvent(sessionVM);
            }
        }

        ///// <summary>
        ///// 获取当前正在显示的Group
        ///// </summary>
        ///// <returns></returns>
        //public virtual SessionGroupVM GetCurrentGroup() { return null; }

        ///// <summary>
        ///// 设置当前正在显示的Gruop
        ///// </summary>
        ///// <param name="groupId"></param>
        //public virtual void SetCurrentGroup(SessionGroupVM sessionGroup) { }
    }
}
