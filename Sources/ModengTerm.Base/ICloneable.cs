using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    public interface ICloneable<T>
    {
        void CopyTo(T dest);
    }
}
