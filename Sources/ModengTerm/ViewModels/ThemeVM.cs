using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class ThemeVM : ViewModelBase
    {
        private string uri;

        public string Uri 
        {
            get { return this.uri; }
            set
            {
                if (this.uri != value) 
                {
                    this.uri = value;
                    this.NotifyPropertyChanged("URI");
                }
            }
        }


        public ThemeVM(WindowTheme theme)
        {
            this.ID = theme.ID;
            this.Name = theme.Name;
            this.Uri = theme.Uri;
        }
    }
}
