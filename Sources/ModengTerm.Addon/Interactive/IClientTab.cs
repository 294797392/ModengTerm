using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 向应用程序公开控制打开的会话窗口的接口
    /// </summary>
    public interface IClientTab
    {
        /// <summary>
        /// 会话Id
        /// </summary>
        object ID { get; }

        /// <summary>
        /// 会话名字
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 会话状态
        /// </summary>
        SessionStatusEnum Status { get; }

        /// <summary>
        /// 会话类型
        /// </summary>
        SessionTypeEnum Type { get; }

        /// <summary>
        /// 读取该会话的选项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">要读取的选项key</param>
        /// <returns></returns>
        T GetOption<T>(string key);
    }

    /// <summary>
    /// 表示打开的终端类型的会话界面
    /// </summary>
    public interface IClientShellTab : IClientTab
    {
        /// <summary>
        /// 提供可以在终端上绘图的接口
        /// </summary>
        IDrawingContext DrawingContext { get; }

        /// <summary>
        /// 向主机发送数据
        /// </summary>
        /// <param name="bytes"></param>
        void Send(byte[] bytes);

        /// <summary>
        /// 向主机发送数据
        /// </summary>
        /// <param name="text"></param>
        void Send(string text);

        VTParagraph GetParagraph(VTParagraphOptions options);

        /// <summary>
        /// 保存指定内容到文件
        /// </summary>
        /// <param name="paragraphType"></param>
        /// <param name="format"></param>
        /// <param name="filePath"></param>
        void SaveToFile(ParagraphTypeEnum paragraphType, ParagraphFormatEnum format, string filePath);

        /// <summary>
        /// 拷贝选中的内容到剪切板
        /// </summary>
        void CopySelection();

        /// <summary>
        /// 清空当前屏幕的内容
        /// </summary>
        void ClearScreen();

        /// <summary>
        /// 对当前视图进行关键字搜索
        /// </summary>
        /// <param name="options">搜索选项</param>
        /// <returns>如果没搜索到，则返回null</returns>
        List<VTextRange> FindMatches(FindOptions options);

        /// <summary>
        /// 获取Ssh引擎
        /// 如果该Tab打开的不是一个Ssh会话，则为空
        /// </summary>
        /// <returns></returns>
        ISshChannel GetSshEngine();
    }

    /// <summary>
    /// 表示打开的Sftp类型的会话界面
    /// </summary>
    public interface IClientFileTransTab : IClientTab
    {

    }
}
