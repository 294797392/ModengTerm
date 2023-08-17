using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 文本元素的基类
    /// 定义了文本元素的一些基础属性，测量信息
    /// </summary>
    public abstract class VTextElement : VTDocumentElement
    {
        /// <summary>
        /// 元素是否需要重新测量
        /// </summary>
        public bool IsMeasureDirty { get; private set; }

        /// <summary>
        /// 元素是否需要重新渲染
        /// 对于VTextLine来说，Render分两步，第一步是对文字进行排版，第二部是画，排版操作是很耗时的
        /// Render的同时也会进行Measure操作
        /// </summary>
        public bool IsRenderDirty { get; private set; }

        /// <summary>
        /// 元素是否需要重新布局
        /// 有可能一个元素需要重新布局，但是不需要重新渲染
        /// 渲染字符会比对字符重新Arrange要慢很多，因为在渲染的时候，系统需要对字符进行排版操作，这个步骤很耗时。为了优化性能，尽可能不去渲染就不要渲染
        /// Arrange本质上就是把排版好了并且渲染好了的字符移动一下位置
        /// </summary>
        public bool IsArrangeDirty { get; private set; }

        /// <summary>
        /// 文本的测量信息
        /// </summary>
        public VTextMetrics Metrics { get; set; }

        /// <summary>
        /// 获取该文本块的宽度
        /// </summary>
        public double Width { get { return this.Metrics.Width; } }

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
        /// 所属的文档
        /// </summary>
        public VTDocument OwnerDocument { get; private set; }

        /// <summary>
        /// 文本样式
        /// </summary>
        public VTextStyle Style { get; set; }

        public VTextElement(VTDocument owner)
        {
            this.OwnerDocument = owner;
            this.Metrics = new VTextMetrics();
        }


        public void SetMeasureDirty(bool isDirty)
        {
            if (this.IsMeasureDirty != isDirty)
            {
                this.IsMeasureDirty = isDirty;
            }
        }

        public void SetRenderDirty(bool isDirty)
        {
            if (this.IsRenderDirty != isDirty)
            {
                this.IsRenderDirty = isDirty;

                // 需要render的时候也说明需要measure
                this.IsMeasureDirty = isDirty;
            }
        }

        public void SetArrangeDirty(bool isDirty)
        {
            if (this.IsArrangeDirty != isDirty)
            {
                this.IsArrangeDirty = isDirty;
            }
        }
    }
}
