using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest
{
    public static class VTDocumentTest
    {
        [UnitTest]
        public static bool SampleTestCase()
        {
            VTDocument document = UnitTestHelper.CreateVTDocument();

            document.Release();

            return true;
        }
    }
}
