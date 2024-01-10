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
        /// 当前显示的文档改变的时候触发
        /// 一个终端有两个文档，分别是主文档（MainDocument）和备用文档（AlternateDocument）
        /// 默认情况下显示主文档，VIM，Man等程序会用到备用文档
        /// 
        /// 第一个VTDocument是oldDocument，意思是切换之前显示的Document
        /// 第二个VTDocument是newDocument，意思是切换之后显示的Document
        /// </summary>
        event Action<IVideoTerminal, VTDocument, VTDocument> DocumentChanged;

        /// <summary>
        /// 会话名字
        /// </summary>
        string Name { get; set; }

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
        /// 创建指定的段落内容
        /// </summary>
        /// <param name="paragraphType">段落类型</param>
        /// <param name="fileType">要创建的内容格式</param>
        /// <returns></returns>
        VTParagraph CreateParagraph(ParagraphTypeEnum paragraphType, LogFileTypeEnum fileType);

        /// <summary>
        /// 获取当前使用鼠标选中的段落区域
        /// </summary>
        /// <returns></returns>
        VTParagraph GetSelectedParagraph();

        /// <summary>
        /// 选中所有内容
        /// </summary>
        void SelectAll();
    }
}