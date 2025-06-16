using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 向应用程序公开控制打开的会话窗口的接口
    /// </summary>
    public interface IHostTab
    {
        /// <summary>
        /// 会话Id
        /// </summary>
        string Id { get; }

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
    }

    /// <summary>
    /// 表示打开的终端类型的会话界面
    /// </summary>
    public interface IShellTab : IHostTab
    {
        void Send(byte[] bytes);
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

        void ClearScreen();
    }

    /// <summary>
    /// 表示打开的Sftp类型的会话界面
    /// </summary>
    public interface ISftpTab : IHostTab
    {

    }
}
