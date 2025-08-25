using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Snippet
{
    public class SnippetVM : ViewModelBase
    {
        private SnippetTypes type;
        private string script;

        public SnippetTypes Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        public string Script
        {
            get { return script; }
            set
            {
                if (this.script != value)
                {
                    this.script = value;
                    this.NotifyPropertyChanged("Script");
                }
            }
        }

        public string TabId { get; private set; }

        public SnippetVM(Snippet snippet)
        {
            this.ID = snippet.Id;
            this.Name = snippet.Name;
            this.Type = snippet.Type;
            this.Script = snippet.Script;
            this.TabId = snippet.TabId;
        }

        public SnippetVM() { }

        public Snippet GetSnippet() 
        {
            return new Snippet() 
            {
                Id = this.ID.ToString(),
                Script = this.Script,
                Name = this.Name,
                Type = this.Type,
                TabId = this.TabId
            };
        }
    }
}
