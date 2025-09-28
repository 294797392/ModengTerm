using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModel.Ftp
{
    /// <summary>
    /// 定义重新加载地址栏的原因
    /// </summary>
    public enum LoadAddressbarReasons
    {
        /// <summary>
        /// 返回上一级目录
        /// </summary>
        OpenParentDirectory,

        /// <summary>
        /// 打开子目录
        /// </summary>
        OpenSubDirectory,
    }
}
