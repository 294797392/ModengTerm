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
            base.Initialize();
        }

        public override void Release()
        {
            base.Release();
        }

        protected override string ReadFile(string filePath)
        {
            throw new NotImplementedException();
        }

        protected override string Execute(string command)
        {
            throw new NotImplementedException();
        }
    }
}
