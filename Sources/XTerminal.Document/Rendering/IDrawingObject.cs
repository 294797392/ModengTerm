using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 封装WPF, Winform里的画图对象
    /// 一个实例对应界面上的一个元素
    /// </summary>
    public interface IDrawingObject
    {
        /// <summary>
        /// 初始化画图对象
        /// </summary>
        void Initialize(VTDocumentElement documentElement);

        /// <summary>
        /// 释放画图对象
        /// </summary>
        void Release();

        /// <summary>
        /// 重绘
        /// </summary>
        /// <param name="documentElement">绘图模型</param>
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
        void Arrange(double x, double y);
    }
}
