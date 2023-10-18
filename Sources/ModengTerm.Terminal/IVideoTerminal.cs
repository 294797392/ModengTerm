using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 提供终端的标准功能接口定义
    /// 不同的功能模块使用该接口去访问终端
    /// </summary>
    public interface IVideoTerminal
    {
        /// <summary>
        /// 当某一行被完整打印之后触发
        /// </summary>
        event Action<IVideoTerminal, VTHistoryLine> LinePrinted;

        /// <summary>
        /// 终端对应的会话信息
        /// </summary>
        XTermSession Session { get; }

        /// <summary>
        /// 终端行大小
        /// </summary>
        int ViewportRow { get; }

        /// <summary>
        /// 终端列大小
        /// </summary>
        int ViewportColumn { get; }

        /// <summary>
        /// 始终处于最上层的Document
        /// </summary>
        IDrawingDocument TopMostCanvas { get; }

        /// <summary>
        /// 当前正在显示的Document
        /// </summary>
        VTDocument ActiveDocument { get; }

        /// <summary>
        /// 获取终端的滚动信息
        /// </summary>
        VTScrollInfo ScrollInfo { get; }

        /// <summary>
        /// 获取UI线程上下文
        /// </summary>
        SynchronizationContext UISyncContext { get; }

        /// <summary>
        /// 保存日志记录器
        /// </summary>
        VTLogger Logger { get; set; }

        /// <summary>
        /// 滚动到某个物理行
        /// </summary>
        /// <param name="physicsRow">要滚动到的物理行数</param>
        /// <param name="options">滚动选项</param>
        void ScrollTo(int physicsRow, ScrollOptions options = ScrollOptions.ScrollToTop);

        /// <summary>
        /// 向SSH主机发送一段文本
        /// </summary>
        /// <param name="text"></param>
        void SendInput(string text);
    }
}