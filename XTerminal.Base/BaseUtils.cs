using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Base
{
    public static class BaseUtils
    {
        public static bool EqualsEx(this List<byte> source, string s)
        {
            if (source.Count != s.Length)
            {
                return false;
            }

            int length = source.Count;
            for (int i = 0; i < length; i++)
            {
                if (source[i] != s[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
