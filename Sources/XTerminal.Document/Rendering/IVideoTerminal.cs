﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 终端显示屏
    /// 显示屏里包含多个Surface，Surface用来真正渲染终端输出
    /// </summary>
    public interface IVideoTerminal
    {
        /// <summary>
        /// 获取相对于整个显示器屏幕的Canvas边界框
        /// </summary>
        VTRect BoundaryRelativeToDesktop { get; }

        /// <summary>
        /// 当用户按下按键的时候要触发这个事件
        /// IVideoTerminal：触发事件的VideoTerminal
        /// VTKeys：用户按下的键盘按键
        /// VTModifierKeys：用户按下的控制按键（ctrl，alt...etc）
        /// string：用户输入的中文字符串，如果没有则写null
        /// </summary>
        event Action<IVideoTerminal, VTInputEvent> InputEvent;

        /// <summary>
        /// 当用户拖动滚动条的时候触发
        /// int:滚动条移动到的行数
        /// </summary>
        event Action<IVideoTerminal, int> ScrollChanged;

        /// <summary>
        /// 鼠标移动的时候触发
        /// 在鼠标移动出Panel外的时候，也会触发
        /// </summary>
        event Action<IVideoTerminal, VTPoint> VTMouseMove;

        /// <summary>
        /// 鼠标按下的时候触发
        /// </summary>
        event Action<IVideoTerminal, VTPoint> VTMouseDown;

        /// <summary>
        /// 鼠标抬起的时候触发
        /// </summary>
        event Action<IVideoTerminal, VTPoint> VTMouseUp;

        /// <summary>
        /// 当鼠标双击的时候触发
        /// </summary>
        event Action<IVideoTerminal, double, double, int> VTMouseDoubleClick;

        /// <summary>
        /// 鼠标滚轮滚动的时候触发
        /// 如果向上滚动则为true，否则为false
        /// </summary>
        event Action<IVideoTerminal, bool> VTMouseWheel;

        /// <summary>
        /// 当终端屏幕大小改变的时候触发
        /// </summary>
        event Action<IVideoTerminal, VTRect> VTSizeChanged;

        /// <summary>
        /// 创建一个画板用来渲染文档
        /// </summary>
        /// <param name="options">渲染选项</param>
        /// <returns></returns>
        IDrawingCanvas CreateCanvas();

        void AddCanvas(IDrawingCanvas canvas);

        void RemoveCanvas(IDrawingCanvas canvas);

        /// <summary>
        /// 获取滚动条信息
        /// </summary>
        /// <param name="maximum">滚动条最大值</param>
        /// <param name="scrollValue">当前滚动条的值</param>
        /// <returns></returns>
        void GetScrollInfo(out int maximum, out int scrollValue);

        /// <summary>
        /// 更新滚动条的信息
        /// </summary>
        /// <param name="maximum">滚动条的最大值</param>
        void SetScrollInfo(int maximum, int scrollValue);

        VTextMetrics MeasureText(string text, double fontSize, string fontFamily);
    }
}