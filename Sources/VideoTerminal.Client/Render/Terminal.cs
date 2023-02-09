using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.TextFormatting;
using XTerminalBase;
using XTerminalController;
using XTerminalParser;
using VideoTerminal.Utility;

namespace XTerminal.Render
{
    /// <summary>
    /// 终端控件
    /// </summary>
    public class Terminal : Panel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("Terminal");

        #endregion

        #region 公开事件

        public event Action<IVideoTerminal, VTInputEvent> InputEvent;

        #endregion

        #region 实例变量

        private Typeface typeface;
        private double pixelPerDip;

        private VTInputEvent inputEvent;

        private Dictionary<int, TextVisual> textVisuals;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.textVisuals.Count; }
        }

        #endregion

        #region 构造方法

        public Terminal()
        {
            this.textVisuals = new Dictionary<int, TextVisual>();
            this.inputEvent = new VTInputEvent();
            this.typeface = new Typeface(new FontFamily("Ya Hei"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            this.pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
        }

        #endregion

        #region 实例方法

        private void NotifyInputEvent(VTInputEvent evt)
        {
            if (this.InputEvent != null)
            {
                this.InputEvent(this,evt);
            }
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 输入中文的时候会触发该事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            //Console.WriteLine(e.Text);
        }

        /// <summary>
        /// 从键盘上按下按键的时候会触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.ImeProcessed)
            {
                // 这些字符交给输入法处理了
            }
            else
            {
                VTKeys vtKey = TerminalUtils.ConvertToVTKey(e.Key);
                this.inputEvent.CapsLock = Console.CapsLock;
                this.inputEvent.Key = vtKey;
                this.inputEvent.Text = null;
                this.inputEvent.Modifiers = VTModifierKeys.None;
                this.NotifyInputEvent(this.inputEvent);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            base.Focus();
        }

        /// <summary>
        /// 参考AvalonEdit
        /// 重写了这个事件后，就会触发鼠标相关的事件
        /// </summary>
        /// <param name="hitTestParameters"></param>
        /// <returns></returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            return this.textVisuals[index];
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Console.WriteLine("Measure");
            return base.MeasureOverride(constraint);
        }

        #endregion

        #region IVideoTerminal

        //public void DrawText(List<VTextBlock> textBlocks)
        //{
        //    foreach (VTextBlock textBlock in textBlocks)
        //    {
        //        this.DrawText(textBlock);
        //    }
        //}

        //public void DrawText(VTextBlock textBlock)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        TextVisual textVisual;
        //        if (!this.textVisuals.TryGetValue(textBlock.Index, out textVisual))
        //        {
        //            textVisual = new TextVisual(textBlock);
        //            textVisual.PixelsPerDip = this.pixelPerDip;
        //            textVisual.Typeface = this.typeface;

        //            this.AddVisualChild(textVisual); // 可视对象的父子关系会影响到命中测试的结果

        //            this.textVisuals[textBlock.Index] = textVisual;
        //        }

        //        textVisual.Draw();
        //    });
        //}

        //public VTextBlockMetrics MeasureText(VTextBlock textBlock)
        //{
        //    FormattedText formattedText = TerminalUtils.CreateFormattedText(textBlock, this.typeface, this.pixelPerDip);
        //    TerminalUtils.UpdateTextMetrics(textBlock, formattedText);
        //    return textBlock.Metrics;
        //}

        //public void PerformAction(VTActions vtAction, params object[] param)
        //{
        //    this.Dispatcher.Invoke(new Action(() =>
        //    {
        //        switch (vtAction)
        //        {
        //            case VTActions.PlayBell:
        //                {
        //                    // 播放响铃
        //                    break;
        //                }

        //            default:
        //                {
        //                    //this.textCanvas.PerformAction(vtAction, param);
        //                    break;
        //                }
        //        }
        //    }));
        //}

        //public ICursorState CursorSaveState()
        //{
        //    logger.WarnFormat("CursorSaveState");
        //    return null;
        //}

        //public void CursorRestoreState(ICursorState state)
        //{
        //    logger.WarnFormat("CursorRestoreState");
        //}

        //public IPresentationDevice CreatePresentationDevice()
        //{
        //    logger.WarnFormat("CreatePresentationDevice");
        //    return null;
        //}

        //public void DeletePresentationDevice(IPresentationDevice device)
        //{
        //    logger.WarnFormat("DeletePresentationDevice");
        //}

        //public bool SwitchPresentationDevice(IPresentationDevice activeDevice)
        //{
        //    logger.WarnFormat("SwitchPresentationDevice");
        //    return true;
        //}

        //public IPresentationDevice GetActivePresentationDevice()
        //{
        //    logger.WarnFormat("GetActivePresentationDevice");
        //    return null;
        //}

        #endregion
    }
}
