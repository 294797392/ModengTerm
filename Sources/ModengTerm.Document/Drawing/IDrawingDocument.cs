using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    public class VTDocumentHandler
    {
        public delegate void MouseWheelDelegate(IDrawingDocument sender, bool upper);
        public delegate void MouseDownDelegate(IDrawingDocument sender, VTPoint location, int clickCount);
        public delegate void MouseMoveDelegate(IDrawingDocument sender, VTPoint location);
        public delegate void MouseUpDelegate(IDrawingDocument sender, VTPoint location);
        public delegate void SizeChangedDelegate(IDrawingDocument sender, VTSize contentSize);
        public delegate void ScrollChangedDelegate(IDrawingDocument sender, int scrollValue);

        public MouseWheelDelegate OnMouseWheel;
        public MouseDownDelegate OnMouseDown;
        public MouseMoveDelegate OnMouseMove;
        public MouseUpDelegate OnMouseUp;
        public SizeChangedDelegate OnSizeChanged;
        public ScrollChangedDelegate OnScrollChanged;
    }

    /// <summary>
    /// 文档渲染对象
    /// 一个文档里包文本，光标等元素
    /// </summary>
    public interface IDrawingDocument
    {
        /// <summary>
        /// 该文档的名字，调试用
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 文档内边距
        /// 内容和边框之间的距离
        /// </summary>
        double PaddingSize { get; set; }

        /// <summary>
        /// 获取文档大小
        /// </summary>
        VTSize Size { get; }

        /// <summary>
        /// 获取文档里的滚动条接口
        /// </summary>
        VTScrollbar Scrollbar { get; }

        /// <summary>
        /// 创建一个绘图对象
        /// </summary>
        /// <param name="type">要创建的绘图对象的类型</param>
        /// <returns>绘图对象</returns>
        IDrawingObject CreateDrawingObject(DrawingObjectTypes type);

        /// <summary>
        /// 删除并释放绘图对象
        /// </summary>
        void DeleteDrawingObject(IDrawingObject drawingObject);

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

        /// <summary>
        /// 为文档增加事件处理器
        /// </summary>
        /// <param name="handler">要增加的事件处理器</param>
        void AddHandler(VTDocumentHandler handler);
    }
}
