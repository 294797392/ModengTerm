using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Base;

namespace VTInterface
{
    /// <summary>
    /// 定义视频终端的标准接口
    /// </summary>
    public interface IVideoTerminal
    {
        /// <summary>
        /// 光标向左移动distance个距离
        /// </summary>
        /// <param name="distance">要移动的距离</param>
        void CursorBackward(int distance);

        /// <summary>
        /// 光标向右边移动distance个距离
        /// </summary>
        /// <param name="distance"></param>
        void CursorForward(int distance);

        /// <summary>
        /// Tab
        /// </summary>
        void ForwardTab();

        /// <summary>
        /// CR - Performs a carriage return.
        /// Moves the cursor to the leftmost column
        /// </summary>
        void CarriageReturn();

        /// <summary>
        /// 换行
        /// </summary>
        void LineFeed();

        /// <summary> 
        /// 执行动作
        /// </summary>
        /// <param name="vtActions">要执行的动作列表</param>
        void PerformAction(List<VTAction> vtActions);

        void PerformAction(VTAction vtAction);

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

        /// <summary>
        /// 隐藏或者显示光标
        /// </summary>
        bool CursorVisibility(bool visible);

        /// <summary>
        /// 移动光标到某行某列
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        void CursorPosition(int row, int column);

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

