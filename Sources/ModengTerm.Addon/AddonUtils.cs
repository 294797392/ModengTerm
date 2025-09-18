using ModengTerm.Base.Metadatas;
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

        /// <summary>
        /// 解决多个插件定义了相同命令的情况
        /// </summary>
        /// <param name="addonId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string GetCommandKey(AddonMetadata metadata, string command) 
        {
            return string.Format("{0}.{1}", metadata.ID, command);
        }
    }
}
