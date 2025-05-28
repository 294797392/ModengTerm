using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons
{
    /// <summary>
    /// 对插件公开会话接口
    /// </summary>
    public interface IAddonSession
    {
        /// <summary>
        /// 会话Id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 会话名字
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 会话状态
        /// </summary>
        SessionStatusEnum Status { get; }
    }

    public interface IShellSession : IAddonSession
    {
        void Send(byte[] bytes);

        VTParagraph GetParagraph(VTParagraphOptions options);
    }
}
