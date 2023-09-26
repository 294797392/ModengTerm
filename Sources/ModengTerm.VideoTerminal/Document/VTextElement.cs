using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 文本元素的基类
    /// 定义了文本元素的一些基础属性，测量信息
    /// </summary>
    public abstract class VTextElement : VTDocumentElement
    {
        /// <summary>
        /// 元素是否需要重新渲染
        /// 对于VTextLine来说，Render分两步，第一步是对文字进行排版，第二部是画，排版操作是很耗时的
        /// Render的同时也会进行Measure操作
        /// </summary>
        private bool renderDirty;

        /// <summary>
        /// 元素是否需要重新测量
        /// </summary>
        private bool isMeasureDirty;

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
            if (this.isMeasureDirty != isDirty)
            {
                this.isMeasureDirty = isDirty;
            }
        }

        public void SetRenderDirty(bool isDirty)
        {
            if (this.renderDirty != isDirty)
            {
                this.renderDirty = isDirty;

                // 需要render的时候也说明需要measure
                this.isMeasureDirty = isDirty;
            }
        }

        #region VTDocumentElement

        public override void RequestInvalidate()
        {
            if (base.arrangeDirty)
            {
                this.DrawingObject.Arrange(this.OffsetX, this.OffsetY);

                base.arrangeDirty = false;
            }

            if (this.renderDirty)
            {
                this.DrawingObject.Draw();

                this.renderDirty = false;
            }

            if (this.isMeasureDirty)
            {
                IDrawingObjectText objectText = this.DrawingObject as IDrawingObjectText;

                objectText.MeasureLine();

                this.isMeasureDirty = false;
            }
        }

        /// <summary>
        /// 测量指定文本里的子文本的矩形框
        /// </summary>
        /// <param name="startIndex">要测量的起始字符索引</param>
        /// <param name="count">要测量的最大字符数，0为全部测量</param>
        /// <returns></returns>
        public VTRect MeasureTextBlock(int startIndex, int count)
        {
            IDrawingObjectText objectText = this.DrawingObject as IDrawingObjectText;

            return objectText.MeasureTextBlock(startIndex, count);
        }

        /// <summary>
        /// 测量一行里某个字符的测量信息
        /// 注意该接口只能测量出来X偏移量，Y偏移量需要外部根据高度自己计算
        /// </summary>
        /// <param name="characterIndex">要测量的字符</param>
        /// <returns>文本坐标，X=文本左边的X偏移量，Y永远是0，因为边界框是相对于该行的</returns>
        public VTRect MeasureCharacter(int characterIndex)
        {
            IDrawingObjectText objectText = this.DrawingObject as IDrawingObjectText;

            return objectText.MeasureCharacter(characterIndex);
        }

        #endregion
    }
}
