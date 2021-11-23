using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
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

        void Print(string text);

        /// <summary>
        /// 光标向前移动distance个距离
        /// </summary>
        /// <param name="distance">要移动的距离</param>
        void CursorBackward(int distance);

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
        /// 图像反色？
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
    }
}
