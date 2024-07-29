using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    /// <summary>
    /// 文档渲染对象
    /// 一个文档里包文本，光标等元素
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// 该文档的名字，调试用
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 获取文档里的滚动条接口
        /// </summary>
        VTScrollbar Scrollbar { get; }

        /// <summary>
        /// 设置文档的可见性
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 获取显示内容区域的像素大小
        /// </summary>
        VTSize ContentSize { get; }

        /// <summary>
        /// 设置文档内容距离文档的边距
        /// </summary>
        double ContentMargin { get; set; }

        /// <summary>
        /// 创建一个绘图对象
        /// </summary>
        /// <param name="type">要创建的绘图对象的类型</param>
        /// <returns>绘图对象</returns>
        IDocumentObject CreateDrawingObject();

        /// <summary>
        /// 删除并释放绘图对象
        /// </summary>
        void DeleteDrawingObject(IDocumentObject drawingObject);

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
