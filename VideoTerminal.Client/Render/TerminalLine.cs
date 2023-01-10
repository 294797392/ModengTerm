using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Render
{
    public class TerminalLine : TerminalVisual
    {
        /// <summary>
        /// 默认的段落宽度
        /// </summary>
        private const int DefaultParagraphWidth = 9999;

        #region 实例变量

        private TextFormatter textFormatter;
        private TerminalTextSource textSource;
        private TerminalTextParagraphProperties textParagraphProperties;

        #endregion

        #region 属性

        /// <summary>
        /// 该行所有的文本段
        /// </summary>
        public List<TerminalLineSegement> SegementList { get; private set; }

        /// <summary>
        /// 该行字符串
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 该行的高度
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// 该行距离Terminal左边的宽度
        /// </summary>
        public double OffsetX { get; private set; }

        /// <summary>
        /// 该行距离Terminal上方的高度
        /// </summary>
        public double OffsetY { get; private set; }

        #endregion

        #region 构造方法

        public TerminalLine()
        {
            this.textFormatter = TextFormatter.Create(TextFormattingMode.Display);
            this.textSource = new TerminalTextSource();
            this.textParagraphProperties = new TerminalTextParagraphProperties();
        }

        public TerminalLine(double offsetX, double offsetY) : 
            this()
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public TerminalLine(double offsetX, double offsetY, string text) :
            this(offsetX, offsetY)
        {
            this.Text = text;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 渲染一次text，并返回渲染后生成的TextLine对象
        /// </summary>
        /// <param name="offsetX">要渲染的字符串的X偏移量</param>
        /// <param name="offsetY">要渲染的字符串的Y偏移量</param>
        /// <param name="text">要渲染的字符串</param>
        /// <returns></returns>
        private TextLine Render(double offsetX, double offsetY, string text)
        {
            this.textSource.Text = text;
            int textSourcePosition = 0;
            Point lineOrigin = new Point(offsetX, offsetY);

            using (DrawingContext dc = base.RenderOpen())
            {
                // 这里保证TextLine一次就可以把所有的字符都渲染完毕
                // DefaultParagraphWidth大于被渲染的字符串的宽度，那么一次就可以渲染完毕
                // 渲染完毕后返回TextLine对象，TextLine对象需要在使用完之后释放掉
                // 如果渲染了多次，那么就会出现多个TextLine对象，那么此时就没法确定先被渲染的TextLine在什么时候释放

                //while (textSourcePosition < this.textSource.Text.Length)
                {
                    TextLine textLine = this.textFormatter.FormatLine(this.textSource, textSourcePosition, DefaultParagraphWidth, this.textParagraphProperties, null);

                    textLine.Draw(dc, lineOrigin, InvertAxes.None);

                    // 更新下一次要渲染的字符的索引
                    textSourcePosition += textLine.Length;

                    // 更新下一次要渲染的字符的位置
                    lineOrigin.X += textLine.Width;

                    return textLine;
                }
            }
        }

        #endregion

        #region 公开接口

        public void AppendText(string text)
        {
            this.Text += text;
        }

        public void PerformRender()
        {
            using (TextLine textLine = this.Render(this.OffsetX, this.OffsetY, this.Text))
            {
                this.Height = textLine.Height;
            }
        }

        #endregion
    }
}
