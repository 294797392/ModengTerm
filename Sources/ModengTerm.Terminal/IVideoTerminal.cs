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
using System.Windows.Controls;

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
        /// 请求改变窗口大小
        /// 
        /// double：窗口宽度增量，可以有负值
        /// double：窗口高度增量，可以有负值
        /// </summary>
        event Action<IVideoTerminal, double, double> RequestChangeWindowSize;

        /// <summary>
        /// 会话名字
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 当前正在显示的Document
        /// </summary>
        VTDocument ActiveDocument { get; }

        /// <summary>
        /// 备用缓冲区
        /// </summary>
        VTDocument AlternateDocument { get; }

        /// <summary>
        /// 主缓冲区
        /// </summary>
        VTDocument MainDocument { get; }

        /// <summary>
        /// 获取当前显示的是否是备用缓冲区
        /// 备用缓冲区没有历史记录
        /// </summary>
        bool IsAlternate { get; }

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
        /// 选中所有内容
        /// </summary>
        void SelectAll();

        /// <summary>
        /// 当大小改变的时候由外部触发
        /// </summary>
        /// <param name="newSize">新的终端显示区域的大小</param>
        void Resize(VTSize newSize);
    }
}