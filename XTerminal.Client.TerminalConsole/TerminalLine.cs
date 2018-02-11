using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace XTerminal.Client.TerminalConsole
{
    public class TerminalLine : DrawingVisual
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalLine");

        #endregion

        #region 实例变量

        private List<TerminalText> texts;
        private double width;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前行的行高
        /// </summary>
        public double Height
        {
            get
            {
                return this.texts.Count == 0 ? 0 : this.texts[0].Height;
            }
        }

        /// <summary>
        /// 获取当前行的宽度
        /// </summary>
        public double Width
        {
            get
            {
                return this.width + this.Margin.Left;
            }
        }

        /// <summary>
        /// 获取当前行的间距
        /// </summary>
        public Thickness Margin { get; set; }

        #endregion

        #region 构造方法

        public TerminalLine()
        {
            this.texts = new List<TerminalText>();
        }

        #endregion

        #region 公开接口

        public void Draw()
        {
            DrawingContext drawCtx = base.RenderOpen();

            Point textPoint = new Point();
            foreach (TerminalText text in this.texts)
            {
                try
                {
                    drawCtx.DrawText(text, textPoint);
                    textPoint.X += text.WidthIncludingTrailingWhitespace;
                }
                catch (Exception ex)
                {
                    logger.Error("TerminalLine.Draw异常", ex);
                }
            }

            drawCtx.Close();
        }

        public void AddText(TerminalText text)
        {
            this.texts.Add(text);
            this.width += text.WidthIncludingTrailingWhitespace;
        }

        #endregion
    }
}