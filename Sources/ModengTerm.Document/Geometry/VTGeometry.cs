using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Geometry
{
    public abstract class VTGeometry : VTElement
    {
        protected VTGeometry(VTDocument ownerDocument) : 
            base(ownerDocument)
        {
        }

        //public abstract void AddGeometry();

        //public abstract void Clear();
    }
}
