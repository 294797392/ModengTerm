using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public enum VTModifierKeys
    { 
        /// <summary>
        /// 没有任何按键按下
        /// </summary>
        None = 0,

        /// <summary>
        /// Shift键
        /// </summary>
        Shift = 4,

        /// <summary>
        /// 在老式的终端设备上叫Meta键，现在是Alt键
        /// </summary>
        MetaAlt = 8,

        /// <summary>
        /// Control键
        /// </summary>
        Control = 16
    }
}
