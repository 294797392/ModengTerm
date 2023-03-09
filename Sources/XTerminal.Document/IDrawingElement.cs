using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 表示一个可以保存绘图状态的对象
    /// </summary>
    public interface IDrawingElement
    {
        /// <summary>
        /// 保存要画的元素的信息
        /// </summary>
        VDocumentElement Element { get; set; }
    }
}
