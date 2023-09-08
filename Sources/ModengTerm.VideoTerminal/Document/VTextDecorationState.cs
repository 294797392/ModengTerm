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
        public VTextDecorations Decoration { get; private set; }

        /// <summary>
        /// 是否至少设置了一次该参数
        /// 如果一次都没设置，那么就不用设置该文本装饰
        /// </summary>
        public bool AlreadySet { get; set; }

        /// <summary>
        /// 文本装饰对应的参数
        /// </summary>
        public object Parameter { get; set; }

        public bool Unset { get; set; }

        public VTextDecorationState(VTextDecorations decoration)
        {
            this.Decoration = decoration;
        }
    }
}
