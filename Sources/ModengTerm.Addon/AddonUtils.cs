using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public static class AddonUtils
    {
        public static string GetObjectId() 
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetCommandKey(string addonId, string command) 
        {
            return string.Format("{0}.{1}", addonId, command);
        }
    }
}
