using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Rendering;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.Document
{
    public class VTDocumentOptions
    {
        /// <summary>
        /// 默认的可视区域行数
        /// 如果SizeMode等于Fixed，那么就使用DefaultViewportRow和DefaultViewportColumn
        /// 如果SizeMode等于AutoFit，那么在VTDocument.Initialize的时候会动态计算行和列
        /// </summary>
        public int ViewportRow { get; set; }

        /// <summary>
        /// 默认的可视区域宽度
        /// </summary>
        public int ViewportColumn { get; set; }

        /// <summary>
        /// 文档所属会话
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 字体信息
        /// </summary>
        public VTypeface Typeface { get; set; }

        /// <summary>
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        public int ScrollDelta { get; set; }

        /// <summary>
        /// 最多保存多少条历史记录
        /// </summary>
        public int ScrollbackMax { get; set; }

        /// <summary>
        /// DECPrivateAutoWrapMode是否启用
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 光标样式
        /// </summary>
        public VTCursorStyles CursorStyle { get; set; }

        /// <summary>
        /// 光标闪烁速度
        /// </summary>
        public VTCursorSpeeds CursorSpeed { get; set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public string CursorColor { get; set; }
    }
}
