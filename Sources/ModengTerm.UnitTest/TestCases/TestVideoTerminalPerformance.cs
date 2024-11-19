using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest.TestCases
{
    /// <summary>
    /// 性能测试
    /// </summary>
    public class TestVideoTerminalPerformance
    {
        [UnitTest]
        public byte[] PrintLine()
        {
            List<byte> bytes = new List<byte>();

            //bytes.AddRange(File.ReadAllBytes("testData"));
            //bytes.AddRange(ControlSequenceGenerator.CRLF());

            bytes.AddRange(Encoding.ASCII.GetBytes(UnitTestHelper.BuildTextLine(400)));
            bytes.AddRange(ControlSequenceGenerator.CRLF());

            return bytes.ToArray();
        }
    }
}
