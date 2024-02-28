using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public interface IFramedElement
    {
        /// <summary>
        /// 显示下一帧的间隔时间
        /// 有可能每一帧之间的间隔都是不一样的
        /// </summary>
        double Delay { get; set; }

        /// <summary>
        /// 还剩余多少时间就渲染
        /// </summary>
        double Elapsed { get; set; }

        /// <summary>
        /// 请求重绘
        /// </summary>
        void RequestInvalidate();
    }

    /// <summary>
    /// 表示一个需要一帧一帧渲染的，实时渲染的元素
    /// </summary>
    public abstract class VTFramedElement<TDrawingObject> : VTElement<TDrawingObject>, IFramedElement
        where TDrawingObject : IDrawingObject
    {
        private double delay;

        /// <summary>
        /// 显示下一帧的间隔时间
        /// 有可能每一帧之间的间隔都是不一样的
        /// </summary>
        public double Delay
        {
            get { return delay; }
            set
            {
                if (delay != value)
                {
                    delay = value;
                }
            }
        }

        /// <summary>
        /// 还剩余多少时间就渲染
        /// </summary>
        public double Elapsed { get; set; }

        public VTFramedElement(IDrawingCanvas drawingDocument) :
            base(drawingDocument)
        {

        }
    }

    public abstract class VTFramedDocumentElement<TDrawingObject> : VTDocumentElement<TDrawingObject>, IFramedElement
        where TDrawingObject : IDrawingObject
    {
        private double delay;

        /// <summary>
        /// 显示下一帧的间隔时间
        /// 有可能每一帧之间的间隔都是不一样的
        /// </summary>
        public double Delay
        {
            get { return delay; }
            set
            {
                if (delay != value)
                {
                    delay = value;
                }
            }
        }

        /// <summary>
        /// 还剩余多少时间就渲染
        /// </summary>
        public double Elapsed { get; set; }

        public VTFramedDocumentElement(VTDocument ownerDocument) :
            base(ownerDocument)
        {

        }
    }
}
