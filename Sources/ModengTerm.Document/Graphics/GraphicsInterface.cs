using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModengTerm.Document.Drawing
{
    /// <summary>
    /// 文档渲染对象
    /// 一个文档里包文本，光标等元素
    /// </summary>
    public interface GraphicsInterface
    {
        event Action<GraphicsInterface, MouseData> GIMouseDown;
        event Action<GraphicsInterface, MouseData> GIMouseUp;
        event Action<GraphicsInterface, MouseData> GIMouseMove;
        event Action<GraphicsInterface, MouseWheelData> GIMouseWheel;

        /// <summary>
        /// 当每次显示到界面上的时候触发
        /// </summary>
        event Action<GraphicsInterface> GILoaded;

        /// <summary>
        /// 当手动滚动滚动条的时候触发
        /// </summary>
        event Action<GraphicsInterface, ScrollChangedData> GIScrollChanged;

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
        /// 获取渲染区域的大小
        /// </summary>
        VTSize DrawAreaSize { get; }

        /// <summary>
        /// 获取是否具有鼠标捕获
        /// </summary>
        bool GIMouseCaptured { get; }

        /// <summary>
        /// 设置文档的内边距（文档渲染区域和文档之间的距离）
        /// </summary>
        /// <param name="padding">内边距</param>
        void SetPadding(double padding);

        /// <summary>
        /// 获取当前按下的修饰键
        /// </summary>
        VTModifierKeys PressedModifierKey { get; }

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

        /// <summary>
        /// 捕获鼠标之后，即使鼠标移出了触发鼠标事件的元素所在区域，也会继续触发该元素的鼠标事件（比如鼠标移动事件）
        /// </summary>
        bool GICaptureMouse();

        /// <summary>
        /// 释放捕获鼠标
        /// </summary>
        void GIRleaseMouseCapture();
    }
}
