using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public class KeywordFilter : LoggerFilter
    {
        public override FilterTypeEnum Type => FilterTypeEnum.PlainText;

        public override bool Filter(string text)
        {
            if (text.Contains(this.FilterText))
            {
                return true;
            }

            return false;
        }
    }
}
