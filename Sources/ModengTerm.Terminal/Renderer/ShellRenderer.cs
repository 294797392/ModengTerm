using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace ModengTerm.Terminal.Renderer
{
    /// <summary>
    /// 定义一个用来解析并渲染终端数据的渲染器
    /// </summary>
    public abstract class ShellRenderer
    {
        protected VideoTerminal videoTerminal;
        protected XTermSession session;

        public ShellRenderer(VideoTerminal vt)
        {
            this.videoTerminal = vt;
            this.session = vt.Session;
        }

        /// <summary>
        /// 初始化渲染器
        /// </summary>
        /// <param name="vt">渲染器所渲染的终端</param>
        /// <param name="session">会话信息</param>
        public abstract void Initialize();

        /// <summary>
        /// 释放渲染器
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// 当用户和主机之间的交互状态改变的时候触发
        /// TODO：当UserInput速度很快的时候，还没来得及收到数据，连续两次触发UserInput
        /// </summary>
        /// <param name="istate">交互状态</param>
        public abstract void OnInteractionStateChanged(InteractionStateEnum istate);

        /// <summary>
        /// 渲染原始数据
        /// </summary>
        /// <param name="bytes">要渲染的原始数据缓冲区</param>
        /// <param name="length">要渲染的原始数据长度</param>
        public abstract void Render(byte[] bytes, int length);
    }
}
