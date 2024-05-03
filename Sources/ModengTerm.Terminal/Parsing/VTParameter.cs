using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Parsing
{
    public static class VTParameter
    {
        public static int GetParameter(List<int> parameters, int index, int defaultParameter)
        {
            if (parameters.Count > index)
            {
                return parameters[index];
            }
            else
            {
                return defaultParameter;
            }
        }
    }
}
