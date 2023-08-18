using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 封装WPF, Winform里的画图对象
    /// </summary>
    public interface IDrawingContext
    {
        /// <summary>
        /// 重绘
        /// </summary>
        void Draw();

        /// <summary>
        /// 设置透明度
        /// 从0 - 1
        /// </summary>
        void SetOpacity(double opacity);

        /// <summary>
        /// 修改此绘图对象的位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Translate(double x, double y);
    }
}
