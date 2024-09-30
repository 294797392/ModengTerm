using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class RecentlySessionVM : ViewModelBase
    {
        public string SessionId { get; set; }

        public RecentlySessionVM(RecentlySession recentlySession)
        {
            this.ID = recentlySession.ID;
            this.Name = recentlySession.SessionName;
            this.SessionId = recentlySession.SessionId;
        }
    }
}
