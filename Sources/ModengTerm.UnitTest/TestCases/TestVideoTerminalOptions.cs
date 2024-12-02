using log4net.Repository.Hierarchy;
using ModengTerm.Document;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest.TestCases
{
    /// <summary>
    /// 对终端的参数做测试
    /// </summary>
    public class TestVideoTerminalOptions
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TestVideoTerminalOptions");

        /// <summary>
        /// 对RollbackMax选项测试
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool RollbackMax()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            int totalLines = document.RollbackMax + row + 100;

            // 打印比最多回滚行数多100行的数据
            List<string> textLines = UnitTestHelper.BuildTextLines(totalLines);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            // 比对历史记录的总行数
            if (history.Lines - row != document.RollbackMax)
            {
                logger.ErrorFormat("history.Lines数值不正确, history.Lines = {0}, rollbackMax = {1}", history.Lines, document.RollbackMax);
                return false;
            }

            // 比对历史记录，此时历史记录应该从第100行开始记录
            if (!UnitTestHelper.CompareHistory(history, textLines.Skip(100).ToList()))
            {
                return false;
            }

            // 比对滚动条的值
            if (scrollInfo.Maximum != document.RollbackMax)
            {
                logger.ErrorFormat("scrollInfo.Maximum数值不正确, scrollInfo.Maximum = {0}, rollbackMax = {1}", scrollInfo.Maximum, document.RollbackMax);
                return false;
            }

            return true;
        }
    }
}
