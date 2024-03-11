using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.TestCase
{
    public class TestCase
    {
        /// <summary>
        /// 测试用例输入数据
        /// </summary>
        public List<UserInput> UserInputs { get; set; }

        /// <summary>
        /// 输入完后所有行的状态
        /// </summary>
        public List<CaseLine> OutputLines { get; set; }

        /// <summary>
        /// 光标所在行数
        /// </summary>
        public int CursorRow { get; set; }

        /// <summary>
        /// 光标所在列数
        /// </summary>
        public int CursorColumn { get; set; }

        public TestCase()
        {
            this.UserInputs = new List<UserInput>();
            this.OutputLines = new List<CaseLine>();
        }
    }
}
