using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public class NoneFilter : LoggerFilter
    {
        public override FilterTypeEnum Type => FilterTypeEnum.None;

        public override bool Filter(string text)
        {
            return true;
        }
    }
}
