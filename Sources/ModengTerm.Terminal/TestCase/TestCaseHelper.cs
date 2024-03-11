using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Document;
using ModengTerm.Terminal.Session;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.TestCase
{
    public class TestCaseHelper
    {
        public ShellSessionVM SessionVM { get; private set; }

        public int Initialize(ShellSessionVM sessionVM)
        {
            this.SessionVM = sessionVM;

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 运行测试用例
        /// </summary>
        /// <param name="testCase"></param>
        /// <returns>是否测试成功</returns>
        public int RunTestCase(TestCase testCase)
        {
            this.SessionVM.Open();

            return ResponseCode.SUCCESS;
        }

        private bool CompareVTextLine(VTextLine textLine1, VTextLine textLine2)
        {
            throw new NotImplementedException();
        }
    }
}
