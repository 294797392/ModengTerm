using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 存储文本数据
    /// </summary>
    public class VTextData
    {
        /// <summary>
        /// 文本字符串
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 文本属性列表
        /// </summary>
        public List<VTextAttribute> Attributes { get; private set; }

        public VTextData()
        {
            this.Text = string.Empty;
            this.Attributes = new List<VTextAttribute>();
        }
    }
}
