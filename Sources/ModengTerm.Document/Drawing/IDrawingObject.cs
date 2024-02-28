using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Document.Drawing
{
    /// <summary>
    /// 封装画图对象
    /// 一个实例对应界面上的一个元素
    /// 该接口保存UI对象的渲染数据，渲染数据由Document模型来生成
    /// 该接口只负责拿到数据后渲染并显示。这样可以最小化移植不同渲染引擎的工作量
    /// </summary>
    public interface IDrawingObject
    {
        /// <summary>
        /// 初始化画图对象
        /// </summary>
        void Initialize();

        /// <summary>
        /// 释放画图对象
        /// </summary>
        void Release();

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
        void Arrange(double x, double y);
    }
}
