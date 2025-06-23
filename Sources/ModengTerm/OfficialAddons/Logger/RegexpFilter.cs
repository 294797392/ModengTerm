using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public class RegexpFilter : LoggerFilter
    {
        public override FilterTypeEnum Type => FilterTypeEnum.Regexp;

        public override bool Filter(string text)
        {
            Match match = Regex.Match(text, this.FilterText);
            if (match.Success)
            {
                return true;
            }

            return false;
        }
    }
}
