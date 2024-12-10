using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest.TestCases
{
    public class TermData
    {
        public int ViewportRow { get; set; }

        public int ViewportColumn { get; set; }
    }

    /// <summary>
    /// 性能测试
    /// </summary>
    public class TestVideoTerminalPerformance
    {
        /* 10000次耗时1100左右 */
        [UnitTest]
        public byte[] PrintLine(TermData termData)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.ASCII.GetBytes(UnitTestHelper.BuildTextLineRandom(termData.ViewportColumn)));
            bytes.AddRange(new byte[] { (byte)'\r', (byte)'\n' });

            return bytes.ToArray();
        }

        [UnitTest]
        public byte[] PrintFile(TermData termData)
        {
            return File.ReadAllBytes("1");
        }
    }
}
