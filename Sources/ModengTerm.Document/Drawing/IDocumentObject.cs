using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Document.Drawing
{
    /// <summary>
    /// 文档里的一个对象实例
    /// 一个实例对应界面上的一个元素
    /// 该接口保存UI对象的渲染数据，渲染数据由Document模型来生成
    /// 该接口只负责拿到数据后渲染并显示。这样可以最小化移植不同渲染引擎的工作量
    /// </summary>
    public interface IDocumentObject
    {
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

        /// <summary>
        /// 画一个矩形
        /// </summary>
        /// <param name="vtRect">矩形的大小和位置</param>
        /// <param name="vtPen">矩形的边框画笔</param>
        /// <param name="backColor">矩形背景色</param>
        void DrawRectangle(VTRect vtRect, VTPen vtPen, VTColor backColor);

        /// <summary>
        /// 一次性画多个矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pen"></param>
        /// <param name="backColor"></param>
        void DrawRectangles(List<VTRect> vtRects, VTPen vtPen, VTColor vtColor);

        /// <summary>
        /// 渲染指定的文本并返回文本的测量信息
        /// </summary>
        /// <param name="formattedText"></param>
        /// <returns>文本的测量信息</returns>
        VTextMetrics DrawText(VTFormattedText vtFormattedText);

        /// <summary>
        /// 测量指定的文本
        /// </summary>
        /// <param name="vtFormattedText"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        VTextRange MeasureText(VTFormattedText vtFormattedText, int startIndex, int count);

        /// <summary>
        /// 清空绘制的图形
        /// </summary>
        void Clear();
    }
}
