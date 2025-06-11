using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using WPFToolkit.MVVM;

namespace ModengTerm.Base
{
    /// <summary>
    /// 存储整个应用程序都需要使用的通用的数据和方法
    /// </summary>
    public class VTApp : ModularApp<VTApp, VTManifest>, INotifyPropertyChanged
    {
        #region 实例变量

        private DispatcherTimer drawFrameTimer;

        #endregion

        #region 属性

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; private set; }

        #endregion

        #region ModularApp

        protected override int OnInitialized()
        {
            this.ServiceAgent = new LocalServiceAgent();
            this.ServiceAgent.Initialize();


            return ResponseCode.SUCCESS;
        }

        protected override void OnRelease()
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        ///// <summary>
        ///// 通知插件有命令触发
        ///// </summary>
        ///// <param name="addonId">插件Id</param>
        ///// <param name="command">命令Id</param>
        //public void RaiseAddonCommand(CommandArgs e)
        //{
        //    //if (string.IsNullOrEmpty(e.AddonId))
        //    //{
        //    //    foreach (AddonContext context in this.AddonContexts)
        //    //    {
        //    //        context.RaiseCommand(e);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    AddonContext context = this.AddonContexts.FirstOrDefault(v => v.Id == e.AddonId);
        //    //    if (context == null) 
        //    //    {
        //    //        logger.ErrorFormat("没有找到对应的插件, {0}", e.AddonId);
        //    //        return;
        //    //    }

        //    //    context.RaiseCommand(e);
        //    //}
        //}

        #endregion

        #region 实例方法

        //private void ProcessFrame(int elapsed, IFramedElement element)
        //{
        //    element.Elapsed -= elapsed;

        //    if (element.Elapsed <= 0)
        //    {
        //        // 渲染
        //        try
        //        {
        //            element.RequestInvalidate();
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error("RequestInvalidate运行异常", ex);
        //        }

        //        element.Elapsed = element.Delay;
        //    }
        //}

        #endregion

        #region 事件处理器

        /// <summary>
        /// 光标闪烁线程
        /// 所有的光标都在这一个线程运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void DrawFrameTimer_Tick(object sender, EventArgs e)
        //{
        //    IEnumerable<ShellSessionVM> vtlist = this.OpenedTerminals;

        //    foreach (ShellSessionVM vt in vtlist)
        //    {
        //        // 如果当前界面上没有显示终端，那么不处理帧
        //        FrameworkElement frameworkElement = vt.Content as FrameworkElement;
        //        if (!frameworkElement.IsLoaded)
        //        {
        //            continue;
        //        }

        //        VTDocument activeDocument = vt.VideoTerminal.ActiveDocument;

        //        int elapsed = this.drawFrameTimer.Interval.Milliseconds;

        //        //this.ProcessFrame(elapsed, activeDocument.Cursor);

        //        //switch (vt.Background.PaperType)
        //        //{
        //        //    case WallpaperTypeEnum.Live:
        //        //        {
        //        //            this.ProcessFrame(elapsed, vt.Background);
        //        //            break;
        //        //        }

        //        //    case WallpaperTypeEnum.Image:
        //        //    case WallpaperTypeEnum.Color:
        //        //        {
        //        //            if (vt.Background.Effect != EffectTypeEnum.None)
        //        //            {
        //        //                this.ProcessFrame(elapsed, vt.Background);
        //        //            }
        //        //            break;
        //        //        }

        //        //    default:
        //        //        {
        //        //            break;
        //        //        }
        //        //}
        //    }
        //}

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
