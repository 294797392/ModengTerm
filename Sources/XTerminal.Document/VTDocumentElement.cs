using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    public abstract class VTDocumentElement
    {
        private bool isDirty;

        /// <summary>
        /// 该文本块左上角的X坐标
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// 该文本块左上角的Y坐标
        /// </summary>
        public double OffsetY { get; set; }

        /// <summary>
        /// 文本的测量信息
        /// </summary>
        public VTElementMetrics Metrics { get; set; }

        /// <summary>
        /// 获取该文本块的宽度
        /// </summary>
        public double Width { get { return this.Metrics.WidthIncludingWhitespace; } }

        /// <summary>
        /// 该行高度，当DECAWM被设置的时候，终端里的一行如果超出了列数，那么会自动换行
        /// 当一行的字符超过终端的列数的时候，DECAWM指令指定了超出的字符要如何处理
        /// DECAWM SET：超出后要在新的一行上从头开始显示字符
        /// DECAWM RESET：超出后在该行的第一个字符处开始显示字符
        /// </summary>
        public double Height { get { return this.Metrics.Height; } }

        /// <summary>
        /// 获取该文本的边界框信息
        /// 在画完之后会更新测量的矩形框信息
        /// </summary>
        public VTRect Bounds { get { return new VTRect(this.OffsetX, this.OffsetY, this.Width, this.Height); } }

        /// <summary>
        /// 该文本块所在行数，从0开始
        /// </summary>
        //public int Row { get; set; }

        /// <summary>
        /// 该数据模型对应的渲染模型
        /// </summary>
        public IDocumentDrawable Drawable { get; private set; }

        public bool IsDirty
        {
            get { return this.isDirty; }
            protected set
            {
                if (this.isDirty != value)
                {
                    this.isDirty = value;
                }
            }
        }

        /// <summary>
        /// 把该元素设置为脏元素
        /// 表示在下次渲染的时候需要重绘
        /// </summary>
        public void SetDirty(bool isDirty)
        {
            if (this.isDirty != isDirty)
            {
                this.isDirty = isDirty;
            }
        }

        public VTDocumentElement()
        {
            this.Metrics = new VTElementMetrics();
        }

        /// <summary>
        /// 关联一个Drawable对象
        /// 该操作会把drawable之前关联的文档模型取消关联
        /// </summary>
        /// <param name="drawable"></param>
        public void AttachDrawable(IDocumentDrawable drawable)
        {
            if (drawable.OwnerElement != null)
            {
                drawable.OwnerElement.DetachDrawable();
            }

            this.Drawable = drawable;
            this.Drawable.OwnerElement = this;
            this.SetDirty(true);
        }

        /// <summary>
        /// 取消关联的Drawable对象
        /// </summary>
        public void DetachDrawable()
        {
            if (this.Drawable != null)
            {
                this.Drawable.OwnerElement = null;
                this.Drawable = null;
            }
        }
    }
}
