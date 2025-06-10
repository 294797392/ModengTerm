using ModengTerm.Base.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel
{
    public class AppThemeVM : ViewModelBase
    {
        private string uri;

        public string Uri
        {
            get { return uri; }
            private set
            {
                if (uri != value)
                {
                    uri = value;
                    this.NotifyPropertyChanged("Uri");
                }
            }
        }

        public string[] Previews { get; private set; }

        public AppThemeVM(AppTheme theme)
        {
            this.ID = theme.ID;
            this.Name = theme.Name;
            Uri = theme.Uri;
            Previews = theme.Previews;
        }
    }
}
