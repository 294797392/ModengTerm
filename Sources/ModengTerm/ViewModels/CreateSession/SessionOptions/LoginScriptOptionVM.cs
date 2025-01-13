using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.CreateSession.SessionOptions
{
    public class ScriptItemVM : ViewModelBase
    {
        private string expect;
        private string send;
        private LineTerminators terminator;

        public string Expect
        {
            get { return this.expect; }
            set
            {
                if (this.expect != value)
                {
                    this.expect = value;
                    this.NotifyPropertyChanged("Expect");
                }
            }
        }

        public string Send
        {
            get { return this.send; }
            set
            {
                if (this.send != value)
                {
                    this.send = value;
                    this.NotifyPropertyChanged("Send");
                }
            }
        }

        public LineTerminators Terminator
        {
            get { return this.terminator; }
            set
            {
                if (this.terminator != value)
                {
                    this.terminator = value;
                    this.NotifyPropertyChanged("Terminator");
                }
            }
        }

        public ScriptItemVM() { }

        public ScriptItemVM(ScriptItem scriptItem) 
        {
            this.ID = scriptItem.ID;
            this.Expect = scriptItem.Expect;
            this.Send = scriptItem.Send;
            this.Terminator = (LineTerminators)scriptItem.Terminator;
        }
    }

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
            scriptItems.AddRange(this.ScriptItems.Select(v => new ScriptItem() { ID = v.ID.ToString(), Expect = v.Expect, Send = v.Send, Terminator = (int)v.Terminator }));
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
