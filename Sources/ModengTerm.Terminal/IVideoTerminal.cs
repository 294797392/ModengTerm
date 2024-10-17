using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Parsing;
using System.Reflection;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 提供终端的标准功能接口定义
    /// 不同的功能模块使用该接口去访问终端
    /// </summary>
    public interface IVideoTerminal
    {
        /// <summary>
        /// 当执行换行之后触发
        /// 主缓冲区和备用缓冲区换行的时候都会触发，使用第二个参数标识当前换行的是主缓冲区还是备用缓冲区
        /// 
        /// IVideoTerminal：终端对象
        /// bool：是否是备用缓冲区
        /// int：换行之前的光标所在物理行数，从0开始计数
        /// VTHistoryLine：被完整打印的行
        /// </summary>
        event Action<IVideoTerminal, bool, int, VTHistoryLine> OnLineFeed;

        /// <summary>
        /// 当前显示的文档改变的时候触发
        /// 一个终端有两个文档，分别是主文档（MainDocument）和备用文档（AlternateDocument）
        /// 默认情况下显示主文档，VIM，Man等程序会用到备用文档
        /// 
        /// 第一个VTDocument是oldDocument，意思是切换之前显示的Document
        /// 第二个VTDocument是newDocument，意思是切换之后显示的Document
        /// </summary>
        event Action<IVideoTerminal, VTDocument, VTDocument> OnDocumentChanged;

        /// <summary>
        /// 当用户通过键盘输入数据的时候触发
        /// 在用户输入之后，发送给主机之前触发
        /// </summary>
        event Action<IVideoTerminal, VTKeyboardInput> OnKeyboardInput;

        /// <summary>
        /// 打印一个字符的时候触发
        /// </summary>
        event Action<IVideoTerminal> OnPrint;

        /// <summary>
        /// 请求改变窗口大小
        /// 
        /// double：窗口宽度增量，可以有负值
        /// double：窗口高度增量，可以有负值
        /// </summary>
        event Action<IVideoTerminal, double, double> RequestChangeWindowSize;

        /// <summary>
        /// 当处理完C0指令之后触发
        /// </summary>
        event Action<IVideoTerminal, ASCIITable> OnC0ActionExecuted;

        /// <summary>
        /// 在渲染结束的时候触发
        /// </summary>
        event Action<IVideoTerminal> OnRendered;

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
        /// 获取终端使用的字体信息
        /// </summary>
        VTypeface Typeface { get; }

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