using ModengTerm.Document;
using ModengTerm.Terminal;

namespace ModengTerm.UnitTest.TestCases
{
    /// <summary>
    /// 调用VideoTerminal或者VTDocument的公开接口
    /// 对VideoTerminal各个功能做测试
    /// </summary>
    public class TestVideoTerminalEngine
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TestVideoTerminalEngine");

        /// <summary>
        /// 测试打印功能
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool Print()
        {
            /* 先打印和可视区域一样的内容，然后判断可视区域内容是否正确 */
            /* 然后再打印10行，判断滚动后的可视区域内容和滚动条的数值是否正确 */

            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            VTDocument mainDocument = terminal.MainDocument;
            VTScrollInfo scrollInfo = mainDocument.Scrollbar;
            int row = mainDocument.ViewportRow;
            int col = mainDocument.ViewportColumn;

            // 打印和可视区域行数一样的内容
            List<string> textLines = UnitTestHelper.BuildTextLines(row);
            UnitTestHelper.DrawTextLines(terminal, textLines);
            // 判断可视区域内容
            if (!UnitTestHelper.CompareDocument(mainDocument, textLines))
            {
                logger.Error("34F9275C-F871-2A07-C1CA-28A32FF7BB0E");
                return false;
            }

            // 此时光标在右下角，先换行
            terminal.ProcessData(new byte[] { (byte)'\r', (byte)'\n' }, 2);

            List<string> textLines1 = UnitTestHelper.BuildTextLines(10);
            UnitTestHelper.DrawTextLines(terminal, textLines1);
            // 判断滚动条，应该是10
            if (scrollInfo.Value != 10)
            {
                logger.Error("{4D1DF256-48F2-4EE5-BA5B-C8A9DA4E1E1A}");
                return false;
            }
            // 判断滚动后的可视区域的内容
            textLines.AddRange(textLines1);
            if (!UnitTestHelper.CompareDocument(mainDocument, textLines.Skip(10).ToList()))
            {
                logger.Error("{862A001F-7570-4F32-8253-C715D8D35821}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 测试当换行保存历史记录之后数据是否正确
        /// 测试包含：
        /// LineFeed
        /// 历史记录
        /// 滚动条数据
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool HistoryRecord()
        {
            /* 打印500行，检查历史记录是否正确，滚动条的位置是否正确 */

            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            int totalLines = 500;

            // 打印100行内容
            List<string> textLines = UnitTestHelper.BuildTextLines(totalLines);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            if (history.Lines != totalLines)
            {
                logger.ErrorFormat("history.Lines != totalLines, {0}, {1}", history.Lines, totalLines);
                return false;
            }

            // 检查历史记录内容是否正确
            if (!UnitTestHelper.CompareHistory(document.History, textLines))
            {
                return false;
            }

            // 检查滚动条的位置是否正确
            int scrollMax = totalLines - row;
            if (!scrollInfo.ScrollAtBottom)
            {
                logger.ErrorFormat("ScrollAtBottom状态不正确");
                return false;
            }

            if (scrollInfo.ScrollAtTop)
            {
                logger.ErrorFormat("scrollInfo.ScrollAtTop状态不正确");
            }

            if (scrollInfo.Maximum != scrollMax)
            {
                logger.ErrorFormat("scrollInfo.Maximum不正确");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 测试滚动相关的功能
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool Scroll()
        {
            /* 打印500行数据，然后滚动到第一行，然后从第一行开始往下再滚动10次，比对可视区域的内容 */

            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            VTDocument document = terminal.MainDocument;
            VTCursor cursor = terminal.Cursor;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            int totalLines = 500;

            List<string> textLines = UnitTestHelper.BuildTextLines(totalLines);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            // 先测试滚动到第1行的数据
            // 使用循环的目的是模拟一行一行滚动
            // 一行一行滚动的处理方式和一次性滚动多行的处理方式不一样
            for (int i = 0; i < 10; i++)
            {
                terminal.ScrollTo(i);
                if (!UnitTestHelper.CompareDocument(document, textLines.Skip(i).Take(row).ToList()))
                {
                    logger.ErrorFormat("ScrollTo({0})数据不正确", i);
                    return false;
                }

                if (document.ActiveLine != null)
                {
                    logger.Error("{BBC100AD-CD16-4C94-880C-81B4FA0007E5}");
                    return false;
                }
            }

            // 测试滚动到第50行
            terminal.ScrollTo(50);
            if (!UnitTestHelper.CompareDocument(document, textLines.Skip(50).Take(row).ToList()))
            {
                logger.ErrorFormat("ScrollTo(50)数据不正确");
                return false;
            }
            if (document.ActiveLine != null)
            {
                logger.Error("{3564D449-C71A-452A-86C5-CE3234AC0564}");
                return false;
            }

            // 滚动条滚动到底
            terminal.ScrollTo(500);
            if (!UnitTestHelper.CompareDocument(document, textLines.TakeLast(row).ToList()))
            {
                logger.ErrorFormat("ScrollTo(500)数据不正确");
                return false;
            }
            if (document.ActiveLine != document.LastLine)
            {
                logger.Error("{CF8169A0-F0FE-4A0E-AB4A-55E629854A9E}");
                return false;
            }

            #region 测试滚动之后的光标位置

            #region 设置物理行号测试逻辑行号

            terminal.ScrollTo(200);
            // 把光标移动到可视区域中间
            document.SetCursorPhysical(210);
            // 判断光标的逻辑行号
            if (cursor.Row != 10)
            {
                logger.Error("{13C28187-51FD-47AF-8099-CFDDFC2AA41A}");
                return false;
            }

            // 往下滚动一行
            terminal.ScrollTo(201);
            // 判断光标物理行号
            if (cursor.PhysicsRow != 210 || cursor.Row != 9)
            {
                logger.Error("{9CE8BA0B-E53C-428A-9CC1-0CF7D394881E}");
                return false;
            }

            // 往上滚动一行
            terminal.ScrollTo(200);
            if (cursor.PhysicsRow != 210 || cursor.Row != 10)
            {
                logger.Error("{A75E7F33-5284-4FE6-A89B-E85DF00F0A01}");
                return false;
            }

            #endregion

            #region 设置逻辑行号测试物理行号

            document.SetCursorLogical(20, cursor.Column);
            if (cursor.PhysicsRow != 220)
            {
                logger.Error("{64ECE4F9-D880-43E0-9E16-B9D2C9D58236}");
                return false;
            }

            #endregion

            #endregion

            return true;
        }

        /// <summary>
        /// 测试改变窗口大小相关的功能
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool WindowResize()
        {
            return false;
        }
    }
}
