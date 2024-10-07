using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels
{
    /// <summary>
    /// 私钥文件
    /// </summary>
    public class PrivateKey : ModelBase
    {
        /// <summary>
        /// 私钥文件内容
        /// </summary>
        public string Content { get; set; }
    }
}
