using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminalController;
using XTerminalParser;

namespace XTerminal.VideoTerminal
{
    public class TextCanvas : Canvas
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TextCanvas");

        #endregion

        #region 实例方法

        private List<TextVisual> textVisuals;

        #endregion

        #region 属性

        public Typeface Typeface { get; set; }

        public double FontSize { get; set; }

        public Brush Foreground { get; set; }

        public double PixelsPerDip { get; set; }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.textVisuals.Count; }
        }

        #endregion

        #region 构造方法

        public TextCanvas()
        {
            this.textVisuals = new List<TextVisual>();
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        public void DrawText(VTextBlock textBlock)
        {
            TextVisual textVisual = new TextVisual(textBlock);
            textVisual.PixelsPerDip = this.PixelsPerDip;
            textVisual.Typeface = this.Typeface;
            textVisual.FontSize = this.FontSize;
            textVisual.Foreground = this.Foreground;

            this.textVisuals.Add(textVisual);

            this.AddVisualChild(textVisual); // 可视对象的父子关系会影响到命中测试的结果

            textVisual.Draw();
        }

        #endregion

        #region 事件处理器

        private void BlinkThreadProc(object state)
        {
            //while (true)
            //{
            //    try
            //    {
            //        this.Dispatcher.Invoke(this.termCaret.Render);
            //    }
            //    catch (Exception ex)
            //    {
            //        // 有可能是线程正在运行的时候然后关闭了客户端
            //        logger.Error("BlinkThreadProc异常", ex);
            //    }

            //    Thread.Sleep(500);
            //}
        }

        #endregion

        #region 重写方法

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            return this.textVisuals[index];
        }

        #endregion
    }
}
