using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 表示被打开的一个会话界面
    /// </summary>
    public interface IPanel
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

        void VisiblePanel(string panelId);
    }

    /// <summary>
    /// 表示打开的终端类型的会话界面
    /// </summary>
    public interface IShellPanel : IPanel
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
    public interface ISftpPanel : IPanel
    {

    }
}
