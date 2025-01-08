using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using System;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 所有PanelContent的基类
    /// </summary>
    public abstract class PanelContentVM : MenuContentVM
    {
        public static readonly string KEY_SERVICE_AGENT = Guid.NewGuid().ToString();
        public static readonly string KEY_OPENED_SESSION = Guid.NewGuid().ToString();

        public override void OnInitialize()
        {
        }

        public override void OnRelease()
        {
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }
    }
}
