using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public class KeywordFilter : LoggerFilter
    {
        public override FilterTypeEnum Type => FilterTypeEnum.Keyword;

        public override bool Filter(string text)
        {
            throw new NotImplementedException();
        }
    }
}
