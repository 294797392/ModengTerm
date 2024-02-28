using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 提供控制滚动条的接口
    /// 该接口负责直接操作UI上的滚动条
    /// </summary>
    public abstract class VTScrollbar
    {
        /// <summary>
        /// 获取或者设置是否显示滚动条
        /// </summary>
        public virtual bool Visible { get; set; }

        /// <summary>
        /// 最多可以滚动到的值
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// 当前滚动到的值
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 可视区域的行数
        /// </summary>
        public int ViewportRow { get; set; }
    }
}
