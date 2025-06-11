using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.UnitTest.Drawing;
using System.Windows.Media.Animation;

namespace ModengTerm.UnitTest.TestCases
{
    /// <summary>
    /// 对终端的参数做测试
    /// </summary>
    public class TestVideoTerminalOptions
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TestVideoTerminalOptions");


        private bool ClickToCursor1()
        {
            /*
             * 光标在第一行打印a，然后鼠标点击第5行，再打印b，然后判断光标位置和历史记录里的内容和当前的文档是否一致 
             */

            XTermSession session = UnitTestHelper.CreateSession(9, 9);
            session.SetOption<bool>(OptionKeyEnum.TERM_ADVANCE_CLICK_TO_CURSOR, true);
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal3(session);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTCursor cursor = document.Cursor;
            VTScrollInfo scrollInfo = document.Scrollbar;

            // 更新VTextLine的边界框信息，便于做命中测试
            document.RequestInvalidate();

            invoker.Print('a');

            // 模拟鼠标点击第5行
            invoker.SimulateMouseDown(4);

            invoker.Print('b');

            if (cursor.Row != 4)
            {
                logger.Error("{3481B0DB-1C77-4A2D-A347-F0B8B3780F8D}");
                return false;
            }

            // 判断前5行的历史记录
            if (!UnitTestHelper.CompareHistory(history, 0, document.FirstLine, 5))
            {
                logger.Error("{27DCEF22-5983-443F-BDBE-C34FB4B474A6}");
                return false;
            }

            return true;
        }

        private bool ClickToCursor2()
        {
            /*
             * 打印20行，滚动到第5行，然后鼠标点击倒数第二行，回车，判断光标；再回车，判断光标位置，历史记录和可视区域内容比对
             */

            XTermSession session = UnitTestHelper.CreateSession(9, 9);
            session.SetOption<bool>(OptionKeyEnum.TERM_ADVANCE_CLICK_TO_CURSOR, true);
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal3(session);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTCursor cursor = document.Cursor;
            VTScrollInfo scrollInfo = document.Scrollbar;

            invoker.PrintLines(20);
            terminal.ScrollTo(5);

            // 鼠标点击倒数第二行
            invoker.SimulateMouseDown(7);
            // 回车
            invoker.LF_FF_VT();
            if (cursor.Row != 8)
            {
                logger.Error("{53084C01-D331-480D-93B0-8B9ED215EE2D}");
                return false;
            }

            // 回车
            invoker.LF_FF_VT();
            if (scrollInfo.Value != 6)
            {
                logger.Error("{9D86CC01-5AEF-46F8-ABF9-A1586106D87B}");
                return false;
            }

            if (cursor.Row != 8)
            {
                logger.Error("{FB7A220A-2D8C-4F21-BB3D-E368EB470662}");
                return false;
            }

            if (!UnitTestHelper.CompareHistory(history, 6, document.FirstLine, 9))
            {
                logger.Error("{7F9E48C4-65F9-4AED-9AF6-BCF224E7485F}");
                return false;
            }

            return true;
        }


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

        [UnitTest]
        public bool Terminal_Advance_AutoWrapMode()
        {
            XTermSession session = UnitTestHelper.CreateSession(9, 9);
            session.SetOption<bool>(OptionKeyEnum.TERM_ADVANCE_AUTO_WRAP_MODE, true);
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal3(session);
            VTDocument document = terminal.MainDocument;

            // 打印比最多回滚行数多100行的数据

            string textLine = UnitTestHelper.BuildTextLine(10);
            UnitTestHelper.DrawTextLine(terminal, textLine);

            List<string> textLines = new List<string>()
            {
                "123456789", "10"
            };

            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{6483E87E-0357-4FF6-88FE-36B2A43ED1A1}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool Terminal_Advance_ClickToCursor()
        {
            if (!this.ClickToCursor1() ||
                !this.ClickToCursor2())
            {
                return false;
            }

            return true;
        }
    }
}
