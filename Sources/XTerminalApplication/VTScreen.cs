using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalDevice
{
    /// <summary>
    /// 表示一个终端屏幕
    /// </summary>
    public class VTScreen
    {
        #region 属性

        /// <summary>
        /// 屏幕的所有行
        /// </summary>
        public List<VTextLine> Lines { get; private set; }

        /// <summary>
        /// 光标所在行
        /// </summary>
        public int CursorRow { get; set; }

        /// <summary>
        /// 光标所在列
        /// </summary>
        public int CursorColumn { get; set; }

        #endregion

        #region 构造方法

        public VTScreen() 
        {
            this.Lines = new List<VTextLine>();
        }

        #endregion
    }
}
