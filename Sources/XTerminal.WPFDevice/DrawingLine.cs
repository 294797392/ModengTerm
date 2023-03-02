using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminalBase;

namespace XTerminal.WPFRenderer
{
    public class DrawingLine : DrawingObject
    {
        private static readonly StringBuilder LineBuilder = new StringBuilder();

        #region 属性

        /// <summary>
        /// 要渲染的textLine对象
        /// </summary>
        public VTextLine TextLine { get; set; }

        #endregion

        protected override void Draw(DrawingContext dc)
        {
            VTextBlock current = this.TextLine.FirstTextBlock;

            LineBuilder.Clear();

            while (current != null)
            {
                // 更新文本测量信息
                TerminalUtils.UpdateTextMetrics(current);

                current = current.Next;
            }

            foreach (VTextBlock textBlock in this.TextLine.TextBlocks)
            {


                //TerminalUtils.UpdateTextMetrics()
                //TerminalUtils.UpdateTextMetrics(textBlock
            }
        }
    }
}
