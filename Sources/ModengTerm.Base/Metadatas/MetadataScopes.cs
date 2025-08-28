using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Metadatas
{
    /// <summary>
    /// 定义插件页面的所属范围
    /// </summary>
    public enum MetadataScopes
    {
        /// <summary>
        /// 侧边栏面板属于客户端，整个app运行期间只有一个实例
        /// </summary>
        Client,

        /// <summary>
        /// 侧边栏面板属于控制台会话
        /// 每个控制台会话都有一个单独的侧边栏面板实例
        /// </summary>
        ConsoleTab,

        /// <summary>
        /// 侧边栏面板属于Ssh会话
        /// 每个Ssh会话都有一个单独的侧边栏面板实例
        /// </summary>
        SshTab,

        /// <summary>
        /// 侧边栏面板属于串口会话
        /// 每个串口会话都有一个单独的侧边栏面板实例
        /// </summary>
        SerialPortTab,

        /// <summary>
        /// 侧边栏面板属于Sftp会话
        /// 每个Sftp会话都有一个单独的侧边栏面板实例
        /// </summary>
        SftpTab,

        /// <summary>
        /// 侧边栏面板属于Tcp会话
        /// 每个Tcp会话都有一个单独的侧边栏面板实例
        /// </summary>
        TcpTab
    }
}
