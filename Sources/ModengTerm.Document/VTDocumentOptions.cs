using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public class VTDocumentOptions
    {
        /// <summary>
        /// 文档名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文档数据模型对应的绘图模型
        /// </summary>
        public IDocument Controller { get; set; }

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
        /// 当一行数据超出了ViewportColumn的时候，是否自动换行
        /// </summary>
        public bool AutoWrapMode { get; set; }

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

        /// <summary>
        /// TextSelection的颜色
        /// </summary>
        public string SelectionColor { get; set; }

        public VTDocumentOptions()
        {
            this.ScrollDelta = 1;
            this.ScrollbackMax = 100;
        }
    }
}
