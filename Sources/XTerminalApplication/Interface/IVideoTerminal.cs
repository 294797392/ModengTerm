﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalBase;
using XTerminalParser;

namespace XTerminalDevice.Interface
{
    /// <summary>
    /// 视频终端渲染器
    /// </summary>
    public interface IVideoTerminal
    {
        #region 属性

        #endregion

        #region 公开接口

        /// <summary>
        /// 当用户按下按键的时候要触发这个事件
        /// IVideoTerminal：触发事件的VideoTerminal
        /// VTKeys：用户按下的键盘按键
        /// VTModifierKeys：用户按下的控制按键（ctrl，alt...etc）
        /// string：用户输入的中文字符串，如果没有则写null
        /// </summary>
        event Action<IVideoTerminal, VTInputEvent> InputEvent;

        /// <summary>
        /// 执行一个虚拟终端的动作
        /// </summary>
        /// <param name="vtAction"></param>
        /// <param name="param"></param>
        void PerformAction(VTActions vtAction, params object[] param);

        /// <summary>
        /// 渲染文本块
        /// </summary>
        /// <param name="textBlocks"></param>
        void DrawText(List<VTextBlock> textBlocks);

        /// <summary>
        /// 渲染文本块
        /// </summary>
        /// <param name="textBlock"></param>
        void DrawText(VTextBlock textBlock);

        /// <summary>
        /// 测量某个文本块的属性
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        VTextBlockMetrics MeasureText(VTextBlock textBlock);

        /// <summary>
        /// 重新调整终端大小
        /// </summary>
        /// <param name="width">终端的宽度</param>
        /// <param name="height">终端高度</param>
        void Resize(double width, double height);

        #endregion

        #region CursorState

        /// <summary>
        /// 保存光标状态
        /// 比方说鼠标的位置..
        /// </summary>
        ICursorState CursorSaveState();

        /// <summary>
        /// 还原鼠标状态
        /// </summary>
        /// <param name="state">要还原的状态</param>
        void CursorRestoreState(ICursorState state);

        #endregion

        #region IPresentationDevice

        /// <summary>
        /// 创建一个新的显示屏
        /// </summary>
        /// <returns></returns>
        IPresentationDevice CreatePresentationDevice();

        /// <summary>
        /// 删除显示屏
        /// </summary>
        /// <param name="screen">要删除的屏幕</param>
        /// <returns></returns>
        void DeletePresentationDevice(IPresentationDevice device);

        /// <summary>
        /// 切换显示屏
        /// </summary>
        /// <param name="activeDevice">要切换到的显示屏</param>
        /// <returns></returns>
        bool SwitchPresentationDevice(IPresentationDevice activeDevice);

        /// <summary>
        /// 获取当前正在使用的显示屏
        /// </summary>
        /// <returns></returns>
        IPresentationDevice GetActivePresentationDevice();

        #endregion
    }
}
