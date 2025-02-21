using ModengTerm.Base.DataModels;
using ModengTerm.Document;
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
    public abstract class RendererBase
    {
        protected VideoTerminal videoTerminal;
        protected XTermSession session;

        public RendererBase(VideoTerminal vt)
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
        /// 渲染从服务器读取的数据
        /// </summary>
        /// <param name="bytes">要渲染的原始数据缓冲区</param>
        /// <param name="length">要渲染的原始数据长度</param>
        public abstract void RenderRead(byte[] bytes, int length);

        /// <summary>
        /// 渲染要写入到服务器的数据
        /// </summary>
        /// <param name="bytes"></param>
        public abstract void RenderWrite(byte[] bytes);


        protected VTextAttributeState CreateForegroundAttribute(string rgbKey)
        {
            VTextAttributeState textAttr = new VTextAttributeState()
            {
                Value = (int)VTextAttributes.Foreground,
                Foreground = VTColor.CreateFromRgbKey(rgbKey)
            };

            return textAttr;
        }
    }
}
