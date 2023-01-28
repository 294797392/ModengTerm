using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalBase;
using XTerminalParser;

namespace XTerminalController
{
    /// <summary>
    /// 定义视频终端的标准接口
    /// </summary>
    public interface IVideoTerminal
    {
        /// <summary>
        /// 当用户按下按键的时候要触发这个事件
        /// </summary>
        event Action<IVideoTerminal, VTInputEventArgs> InputEvent;

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

        void DrawText(VTextBlock textBlock);

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

