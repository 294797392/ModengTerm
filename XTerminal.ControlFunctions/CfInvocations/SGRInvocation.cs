using AsciiControlFunctions.CfInvocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlFunctions.CfInvocations
{
    /// <summary>
    /// SELECT GRAPHIC RENDITION 
    /// SGR is used to establish one or more graphic rendition aspects for subsequent text. The established 
    /// aspects remain in effect until the next occurrence of SGR in the data stream, depending on the setting of
    /// the GRAPHIC RENDITION COMBINATION MODE(GRCM)
    /// </summary>
    public struct SGRInvocation : ICfInvocation
    {
        /// <summary>
        /// 文本装饰
        /// </summary>
        public List<TextDecorationEnum> Decorations;

        /// <summary>
        /// 要显示的文本
        /// </summary>
        public string Text;

        /// <summary>
        /// 前景色
        /// </summary>
        public string Foreground;

        /// <summary>
        /// 背景色
        /// </summary>
        public string Background;

        /// <summary>
        /// 文字装饰
        /// </summary>
        public enum TextDecorationEnum
        {
            Bright,
            Dim,
            Underscore,
            Blink,
            Reverse,
            Hidden,
            ResetAllAttributes
        }
    }
}