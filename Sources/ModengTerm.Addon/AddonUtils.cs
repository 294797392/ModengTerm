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
    }
}
