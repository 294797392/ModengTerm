using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Rendering
{
    public class VTEventHandler
    {
        public delegate void MouseWheelDelegate(bool upper);
        public delegate void MouseDownDelegate(VTPoint location, int clickCount);
        public delegate void MouseMoveDelegate(VTPoint location);
        public delegate void MouseUpDelegate(VTPoint location);
        public delegate void SizeChangedDelegate(VTSize contentSize);
        public delegate void ScrollChangedDelegate(int scrollValue);

        public MouseWheelDelegate OnMouseWheel;
        public MouseDownDelegate OnMouseDown;
        public MouseMoveDelegate OnMouseMove;
        public MouseUpDelegate OnMouseUp;
        public SizeChangedDelegate OnSizeChanged;
        public ScrollChangedDelegate OnScrollChanged;
    }

    /// <summary>
    /// 表示一个用来渲染终端输出的表面
    /// </summary>
    public interface IDrawingDocument
    {
        /// <summary>
        /// 该文档的名字，调试用
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 该文档的内容距离边框的距离
        /// </summary>
        double ContentMargin { get; set; }

        /// <summary>
        /// 获取文档里的滚动条
        /// </summary>
        IDrawingScrollbar Scrollbar { get; }

        /// <summary>
        /// 设置滚动条的可见性
        /// </summary>
        bool ScrollbarVisible { get; set; }

        /// <summary>
        /// 创建一个渲染对象
        /// </summary>
        /// <returns>绘图对象</returns>
        TDrawingObject CreateDrawingObject<TDrawingObject>(VTDocumentElements type) where TDrawingObject : IDrawingObject;

        /// <summary>
        /// 删除并释放渲染对象
        /// </summary>
        void DeleteDrawingObject(IDrawingObject drawingObject);

        /// <summary>
        /// 删除所有的渲染对象
        /// </summary>
        void DeleteDrawingObjects();

        /// <summary>
        /// Document由ContentArea和Scrollbar组成
        /// 这个函数的作用是获取相对于整个Document，ContentArea的大小（也就是显示文档的区域，不包含滚动条和其他的区域）
        /// </summary>
        /// <returns></returns>
        VTRect GetContentRect();

        /// <summary>
        /// 注册该文档的事件
        /// </summary>
        /// <param name="eventHandler"></param>
        void AddEventHandler(VTEventHandler eventHandler);
    }
}
