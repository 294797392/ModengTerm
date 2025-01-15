using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.RightsManagement;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession.SessionOptions
{
    public class LoginScriptOptionVM : OptionContentVM
    {
        public BindableCollection<ScriptItemVM> ScriptItems { get; private set; }

        public override void LoadOptions(XTermSession session)
        {
            List<ScriptItem> scriptItems = session.GetOption<List<ScriptItem>>(OptionKeyEnum.LOGIN_SCRIPT_ITEMS, new List<ScriptItem>());
            this.ScriptItems.AddRange(scriptItems.Select(v => new ScriptItemVM(v)));
        }

        public override bool SaveOptions(XTermSession session)
        {
            List<ScriptItem> scriptItems = new List<ScriptItem>();

            foreach (ScriptItemVM sivm in this.ScriptItems)
            {
                if (string.IsNullOrEmpty(sivm.Expect) && string.IsNullOrEmpty(sivm.Send) && sivm.Terminator == LineTerminators.None)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(sivm.Expect))
                {
                    MTMessageBox.Info("请输入提示符");
                    return false;
                }

                ScriptItem scriptItem = sivm.ToScriptItem();

                scriptItems.Add(scriptItem);
            }

            session.SetOption<List<ScriptItem>>(OptionKeyEnum.LOGIN_SCRIPT_ITEMS, scriptItems);

            return true;
        }

        public override void OnInitialize()
        {
            this.ScriptItems = new BindableCollection<ScriptItemVM>();
        }

        public override void OnLoaded()
        {
        }

        public override void OnRelease()
        {
        }

        public override void OnUnload()
        {
        }
    }
}
