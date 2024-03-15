using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
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
        /// 当文档被滚动的时候触发
        /// int:oldValue
        /// int:newValue
        /// </summary>
        event Action<IVideoTerminal, int, int> ScrollChanged;

        /// <summary>
        /// 会话名字
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 当前正在显示的Document
        /// </summary>
        VTScreen ActiveScreen { get; }

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
        /// <param name="formatType">要创建的段落格式</param>
        /// <returns></returns>
        VTParagraph CreateParagraph(ParagraphTypeEnum paragraphType, ParagraphFormatEnum formatType);

        /// <summary>
        /// 获取当前使用鼠标选中的段落区域
        /// </summary>
        /// <returns></returns>
        VTParagraph GetSelectedParagraph();

        /// <summary>
        /// 选中所有内容
        /// </summary>
        void SelectAll();

        /// <summary>
        /// 找到历史行
        /// </summary>
        /// <param name="physicsRow">要找的历史行的物理行号</param>
        /// <param name="historyLine">找到了的历史行</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool TryGetHistoryLine(int physicsRow, out VTHistoryLine historyLine);
    }
}