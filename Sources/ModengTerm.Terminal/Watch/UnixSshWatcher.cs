using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class UnixSshWatcher : UnixWatcher
    {
        public UnixSshWatcher(XTermSession session) :
            base(session)
        {
        }

        public override void Initialize()
        {
        }

        public override void Release()
        {
        }


        public override string df_h()
        {
            throw new NotImplementedException();
        }

        public override string proc_meminfo()
        {
            throw new NotImplementedException();
        }

        public override string proc_stat()
        {
            throw new NotImplementedException();
        }
    }
}
