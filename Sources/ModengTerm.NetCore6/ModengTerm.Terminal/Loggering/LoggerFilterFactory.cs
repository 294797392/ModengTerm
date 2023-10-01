using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public static class LoggerFilterFactory
    {
        public static LoggerFilter Create(FilterTypeEnum filterType)
        {
            switch (filterType)
            {
                case FilterTypeEnum.None: return new NoneFilter();
                case FilterTypeEnum.Keyword: return new KeywordFilter();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
