using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 存储文本装饰的状态（设置或者未设置）
    /// </summary>
    public class VTextDecorationState
    {
        /// <summary>
        /// 文本装饰类型
        /// </summary>
        public VTextDecorationEnum Decoration { get; private set; }

        /// <summary>
        /// 文本装饰对应的参数
        /// </summary>
        public object Parameter { get; set; }

        public bool Unset { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        public VTextDecorationState(VTextDecorationEnum decoration)
        {
            this.Decoration = decoration;
        }
    }
}
