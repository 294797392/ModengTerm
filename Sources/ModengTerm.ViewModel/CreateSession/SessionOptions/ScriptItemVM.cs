using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.CreateSession.SessionOptions
{
    public class ScriptItemVM : ViewModelBase
    {
        private string expect;
        private string send;
        private LineTerminators terminator;

        public string Expect
        {
            get { return expect; }
            set
            {
                if (expect != value)
                {
                    expect = value;
                    this.NotifyPropertyChanged("Expect");
                }
            }
        }

        public string Send
        {
            get { return send; }
            set
            {
                if (send != value)
                {
                    send = value;
                    this.NotifyPropertyChanged("Send");
                }
            }
        }

        public LineTerminators Terminator
        {
            get { return terminator; }
            set
            {
                if (terminator != value)
                {
                    terminator = value;
                    this.NotifyPropertyChanged("Terminator");
                }
            }
        }

        public ScriptItemVM() { }

        public ScriptItemVM(ScriptItem scriptItem)
        {
            this.ID = scriptItem.ID;
            Expect = scriptItem.Expect;
            Send = scriptItem.Send;
            Terminator = (LineTerminators)scriptItem.Terminator;
        }

        public ScriptItem ToScriptItem()
        {
            ScriptItem scriptItem = new ScriptItem()
            {
                ID = this.ID.ToString(),
                Expect = Expect,
                Send = Send,
                Terminator = (int)Terminator
            };

            return scriptItem;
        }
    }
}
