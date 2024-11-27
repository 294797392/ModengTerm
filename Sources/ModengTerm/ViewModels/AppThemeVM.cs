using ModengTerm.Base.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class AppThemeVM : ViewModelBase
    {
        private string uri;

        public string Uri
        {
            get { return this.uri; }
            private set
            {
                if (this.uri != value)
                {
                    this.uri = value;
                    this.NotifyPropertyChanged("Uri");
                }
            }
        }

        public string[] Previews { get; private set; }

        public AppThemeVM(AppTheme theme)
        {
            this.ID = theme.ID;
            this.Name = theme.Name;
            this.Uri = theme.Uri;
            this.Previews = theme.Previews;
        }
    }
}
