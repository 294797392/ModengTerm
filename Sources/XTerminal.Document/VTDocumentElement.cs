using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 指定文档上的元素类型
    /// </summary>
    public enum VTDocumentElements
    {
        TextLine,

        Cursor,

        SelectionRange
    }

    /// <summary>
    /// 表示文档上的一个元素
    /// </summary>
    public abstract class VTDocumentElement
    {
        /// <summary>
        /// 该对象的类型
        /// </summary>
        public abstract VTDocumentElements Type { get; }

        /// <summary>
        /// 用来保存不同平台的绘图上下文信息
        /// </summary>
        public object DrawingContext { get; set; }

        /// <summary>
        /// 元素是否需要重新测量
        /// </summary>
        public bool IsMeasureDirety { get; private set; }

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
        //public bool IsArrangeDirty { get; private set; }


        /// <summary>
        /// 该元素左上角的X坐标
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// 该元素左上角的Y坐标
        /// </summary>
        public double OffsetY { get; set; }

        public VTDocumentElement()
        {
        }

        public void SetMeasureDirty(bool isDirty)
        {
            if (this.IsMeasureDirety != isDirty)
            {
                this.IsMeasureDirety = isDirty;
            }
        }

        public void SetRenderDirty(bool isDirty)
        {
            if (this.IsRenderDirty != isDirty)
            {
                this.IsRenderDirty = isDirty;

                // 需要render的时候也说明需要measure
                this.IsMeasureDirety = isDirty;
            }
        }
    }
}
