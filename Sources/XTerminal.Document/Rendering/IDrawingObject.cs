using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 存储界面上渲染对象的状态信息
    /// 对应WPF的DrawingObject实现
    /// </summary>
    public interface IDrawingObject
    {
        string ID { get; }

        /// <summary>
        /// 存储要渲染的对象
        /// </summary>
        VTDrawable Drawable { get; set; }
    }
}
