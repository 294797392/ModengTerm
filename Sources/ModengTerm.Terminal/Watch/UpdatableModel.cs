using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class UpdatableModel
    {
        private string id;

        public string ID 
        {
            get { return this.id; }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                }
            }
        }
    }
}
