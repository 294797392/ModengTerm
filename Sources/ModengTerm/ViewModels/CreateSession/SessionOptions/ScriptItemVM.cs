using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ScriptItem ToScriptItem()
        {
            ScriptItem scriptItem = new ScriptItem()
            {
                ID = this.ID.ToString(),
                Expect = this.Expect,
                Send = this.Send,
                Terminator = (int)this.Terminator
            };

            return scriptItem;
        }
    }
}
