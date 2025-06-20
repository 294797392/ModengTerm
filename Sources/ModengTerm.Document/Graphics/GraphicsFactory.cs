using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModengTerm.Document.Graphics
{
    /// <summary>
    /// 定义文档的渲染接口
    /// 一个文档里包文本，光标等元素
    /// </summary>
    public interface GraphicsFactory
    {
        /// <summary>
        /// 获取终端的大小
        /// 如果存在Padding，那么终端比显示区域小
        /// </summary>
        VTSize TerminalSize { get; }

        /// <summary>
        /// 获取文档里的滚动条接口
        /// </summary>
        GraphicsScrollbar GetScrollbarDrawingObject();

        /// <summary>
        /// 创建一个绘图对象
        /// </summary>
        /// <param name="type">要创建的绘图对象的类型</param>
        /// <returns>绘图对象</returns>
        GraphicsObject CreateDrawingObject();

        /// <summary>
        /// 删除并释放绘图对象
        /// </summary>
        void DeleteDrawingObject(GraphicsObject drawingObject);

        /// <summary>
        /// 删除所有的绘图对象
        /// </summary>
        void DeleteDrawingObjects();

        /// <summary>
        /// 获取字形信息
        /// </summary>
        /// <param name="textStyle">字体样式</param>
        /// <returns></returns>
        VTypeface GetTypeface(double fontSize, string fontFamily);
    }
}
