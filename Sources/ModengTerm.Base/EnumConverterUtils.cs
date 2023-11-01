using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    public static class EnumConverterUtils
    {
        public static bool CheckValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            string v = value.ToString();
            if (string.IsNullOrEmpty(v))
            {
                return false;
            }

            return true;
        }
    }
}
