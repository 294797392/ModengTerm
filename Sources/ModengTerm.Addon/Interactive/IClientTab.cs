using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Graphics;
using System.Windows.Media;

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
        /// <param name="optionKey">要读取的选项key</param>
        /// <param name="defaultValue">如果该选项不存在，指定默认值</param>
        /// <returns></returns>
        T GetOption<T>(OptionKeyEnum key, T defaultValue);

        /// <summary>
        /// 存储Tab相关的数据
        /// </summary>
        /// <param name="addon"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetData(AddonModule addon, string key, object value);

        /// <summary>
        /// 获取Tab相关的数据
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="addon"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetData<TValue>(AddonModule addon, string key);
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

        IOverlayPanel GetOverlayPanel(string id);

        /// <summary>
        /// 对当前视图进行关键字搜索
        /// </summary>
        /// <param name="options"></param>
        /// <returns>如果没搜索到，则返回null</returns>
        List<VTextRange> FindMatches(FindOptions options);
    }

    /// <summary>
    /// 表示打开的Sftp类型的会话界面
    /// </summary>
    public interface IClientSftpTab : IClientTab
    {

    }
}
