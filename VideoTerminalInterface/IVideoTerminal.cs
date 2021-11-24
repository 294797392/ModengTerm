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
        /// 播放响铃
        /// </summary>
        void WarningBell();

        /// <summary>
        /// 打印一个字符
        /// </summary>
        /// <param name="c"></param>
        void Print(char c);

        /// <summary>
        /// 打印一个字符串
        /// </summary>
        /// <param name="text"></param>
        void Print(string text);

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

        void SetDefaultForeground();
        void SetDefaultBackground();
        void SetDefaultAttributes();
        void SetBold(bool bold);
        /// <summary>
        /// 降低颜色强度
        /// </summary>
        /// <param name="faint"></param>
        void SetFaint(bool faint);
        /// <summary>
        /// 字体设置为斜体
        /// </summary>
        /// <param name="italics"></param>
        void SetItalics(bool italics);

        /// <summary>
        /// 设置光标是否闪烁
        /// </summary>
        /// <param name="blinking"></param>
        void SetBlinking(bool blinking);

        /// <summary>
        /// 隐藏字符
        /// </summary>
        /// <param name="invisible">是否隐藏字符</param>
        void SetInvisible(bool invisible);

        /// <summary>
        /// 仍可读但标记为可删除的字符
        /// </summary>
        /// <param name="crossedOut"></param>
        void SetCrossedOut(bool crossedOut);

        /// <summary>
        /// 反转前景和背景属性
        /// terminal项目使用consoleAPI实现，百度搜索关键字：COMMON_LVB_REVERSE_VIDEO
        /// </summary>
        /// <param name="reverse"></param>
        void SetReverseVideo(bool reverse);

        /// <summary>
        /// 设置文字下划线
        /// </summary>
        /// <param name="underline"></param>
        void SetUnderline(bool underline);

        /// <summary>
        /// 设置文字双下划线
        /// </summary>
        /// <param name="underline"></param>
        void SetDoublyUnderlined(bool underline);

        /// <summary>
        /// 设置上划线
        /// </summary>
        /// <param name="overline"></param>
        void SetOverlined(bool overline);

        /// <summary>
        /// 设置字体颜色
        /// </summary>
        /// <param name="color"></param>
        void SetIndexedForeground(TextColor color);

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        /// <param name="color"></param>
        void SetIndexedBackground(TextColor color);

        void SetForeground(byte r, byte g, byte b);
        void SetBackground(byte r, byte g, byte b);

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

