using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest
{
    public static class UnitTestHelper
    {
        public static VTDocument CreateVTDocument()
        {
            VTDocumentOptions options = new VTDocumentOptions() { };

            VTDocument document = new VTDocument(options);
            document.Initialize();
            return document;
        }
    }
}
