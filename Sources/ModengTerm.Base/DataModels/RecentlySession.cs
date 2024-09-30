using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    public class RecentlySession : ModelBase
    {
        public string SessionId { get; set; }

        public string SessionName { get; set; }
    }
}
