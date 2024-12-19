using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;

namespace ModengTerm.UnitTest.TestCases
{
    /// <summary>
    /// 模拟SshServer，手动生成原始的控制序列让终端处理，然后判断终端渲染之后的数据是否正确
    /// 每个指令除了测试自己，还要测试受自身影响的其他指令
    /// 验证方式：把C#生成的指令在C++控制台程序里编译，然后用WindowsTerminal运行看最终效果是否和ModengTerm一致
    /// </summary>
    public class TestVideoTerminalAction
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TestVideoTerminalAction");

        private bool DECSTBM_SetScrollingRegion_LineFeed()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            int oldScrollValue = scrollInfo.Value;
            int oldScrollMax = scrollInfo.Maximum;

            List<string> textLines = UnitTestHelper.BuildTextLines(row);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region LineFeed

            /* 先设置滚动边距，然后在最后一行滚动，最后比对 */

            /* marginTop = 1, marginBottom = 2 */

            invoker.DECSTBM_SetScrollingRegion(2, row - 2);
            invoker.CUP_CursorPosition(7, 0);
            // 在滚动区域内最后一行执行LineFeed动作，此时滚动区域内最后一行应该为空
            invoker.CRLF();

            List<string> textLines2 = new List<string>() { "1", "3", "4", "5", "6", "7", "", "8", "9" };
            List<string> textLines3 = UnitTestHelper.BuildTextLines(document);

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{7C858FAE-493B-4CC9-AAC6-361356D46AC2}");
                return false;
            }

            #endregion

            // 保证滚动条不变
            int newScrollValue = scrollInfo.Value;
            int newScrollMax = scrollInfo.Maximum;

            if (newScrollValue != oldScrollValue || newScrollMax != oldScrollMax)
            {
                logger.Error("{CC5DFE1C-E627-4B43-91F8-A5511288810B}");
                return false;
            }

            return true;
        }

        private bool DECSTBM_SetScrollingRegion_RI_ReserveLine()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            int oldScrollValue = scrollInfo.Value;
            int oldScrollMax = scrollInfo.Maximum;

            List<string> textLines = UnitTestHelper.BuildTextLines(row);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region RI_ReserveLine

            /* marginTop = 1, marginBottom = 2 */

            invoker.DECSTBM_SetScrollingRegion(2, 7);
            invoker.CUP_CursorPosition(2, 1);
            invoker.RI_ReverseLineFeed();

            List<string> textLines2 = new List<string>() { "1", "", "2", "3", "4", "5", "6", "8", "9" };
            List<string> textLines3 = UnitTestHelper.BuildTextLines(document);

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{45D08A62-0031-4DB1-96F0-70779B51CE81}");
                return false;
            }

            #endregion

            // 保证滚动条不变
            int newScrollValue = scrollInfo.Value;
            int newScrollMax = scrollInfo.Maximum;

            if (newScrollValue != oldScrollValue || newScrollMax != oldScrollMax)
            {
                logger.Error("{42BA0D02-7AAA-460E-AE47-B149A46A1974}");
                return false;
            }

            return true;
        }

        private bool DECSTBM_SetScrollingRegion_SD_ScrollDown()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(5, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;

            List<string> textLines = UnitTestHelper.BuildTextLines(5);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 先设置滚动边距，然后执行SD_ScrollDown，最后比对 */

            /* marginTop = 1, marginBottom = 1 */
            invoker.DECSTBM_SetScrollingRegion(2, 5 - 1);
            invoker.SD_ScrollDown(2);

            List<string> textLines2 = new List<string>() { "1", "", "", "2", "5" };
            List<string> textLines3 = UnitTestHelper.BuildTextLines(document);

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{8C7A0E81-776D-43B6-BBDB-50937E0D1577}");
                return false;
            }

            return true;
        }

        private bool DECSTBM_SetScrollingRegion_SU_ScrollUp()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(5, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            byte[] buffer = new byte[0];

            List<string> textLines = UnitTestHelper.BuildTextLines(5);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 先设置滚动边距，然后执行SD_ScrollUp，最后比对 */

            /* marginTop = 1, marginBottom = 1 */
            invoker.DECSTBM_SetScrollingRegion(2, 5 - 1);
            invoker.SU_ScrollUp(2);

            List<string> textLines2 = new List<string>() { "1", "4", "", "", "5" };
            List<string> textLines3 = UnitTestHelper.BuildTextLines(document);

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{4BF1251C-9743-45C7-AC2A-FBE8EF35970F}");
                return false;
            }

            return true;
        }

        private bool LineFeed_Alternate()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.AlternateDocument;
            VTCursor cursor = document.Cursor;

            invoker.ASB_AlternateScreenBuffer(true);


            invoker.PrintLines(9);
            List<string> textLines = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            if (!UnitTestHelper.CompareDocument2(document, textLines))
            {
                logger.Error("{F67BAA89-58BE-48A0-B086-393A481BE1A6}");
                return false;
            }

            // 滚动备用缓冲区然后比对
            invoker.CRLF();
            invoker.Print('1');
            List<string> textLines2 = new List<string>() { "2", "3", "4", "5", "6", "7", "8", "9", "1" };
            if (!UnitTestHelper.CompareDocument2(document, textLines2))
            {
                logger.Error("{979F0CF9-71FD-4DF1-A102-18CBA7C64F33}");
                return false;
            }

            if (document.ActiveLine != document.LastLine)
            {
                logger.Error("{F773EBEE-549A-4BDE-9CCE-7A8F5BA86BFB}");
                return false;
            }

            return true;
        }

        private bool RI_ReverseLineFeed1() 
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTCursor cursor = document.Cursor;

            invoker.PrintLines(9);
            List<string> textLines = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            if (!UnitTestHelper.CompareDocument2(document, textLines))
            {
                logger.Error("{2649A51D-FC9F-4BCA-B8BC-4893546BE60A}");
                return false;
            }

            // 光标在第一行，往上滚动一行
            invoker.CUP_CursorPosition(1, 1);
            invoker.RI_ReverseLineFeed();
            List<string> textLines2 = new List<string>() { "", "1", "2", "3", "4", "5", "6", "7", "8" };
            if (!UnitTestHelper.CompareDocument2(document, textLines2))
            {
                logger.Error("{3FD14C12-9D24-4310-994F-43DE3AD15679}");
                return false;
            }

            if (document.ActiveLine != document.FirstLine)
            {
                logger.Error("{D047574A-0C0A-47A5-BE21-A9678E45C0E4}");
                return false;
            }

            return true;
        }

        private bool RI_ReverseLineFeed2()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTCursor cursor = document.Cursor;

            invoker.PrintLines(9);

            // 光标在第一行，往上滚动一行
            invoker.RI_ReverseLineFeed();

            if (document.ActiveLine != document.LastLine.PreviousLine)
            {
                logger.Error("{D5F22F26-5C07-4E72-AA02-1EEDBF4F3705}");
                return false;
            }

            return true;
        }

        private VTextLine FindLogicalRow(VTDocument document, int logicalRow)
        {
            VTextLine current = document.FirstLine;

            for (int i = 0; i < logicalRow; i++)
            {
                current = current.NextLine;
            }

            return current;
        }

        #region 针对于每个指令做单元测试

        [UnitTest]
        public bool Print()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument mainDocument = terminal.MainDocument;
            VTScrollInfo scrollInfo = mainDocument.Scrollbar;
            int row = mainDocument.ViewportRow;
            int col = mainDocument.ViewportColumn;

            #region 直接打印到第5列

            /* 当前行里的VTCharacters是空的，光标直接移动到第5列去打印一个字符A */

            invoker.CUP_CursorPosition(1, 5);
            invoker.Print('A');

            string firstLine = VTUtils.CreatePlainText(mainDocument.FirstLine.Characters);
            if (firstLine != "    A")
            {
                logger.ErrorFormat("15595312-F6A2-72B7-5901-1DBE0768BEDA");
                return false;
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 定位光标
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool CUP_CursorPosition()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.ActiveDocument;
            VTCursor cursor = document.Cursor;

            invoker.CUP_CursorPosition(2, 1);

            if (cursor.Row != 1)
            {
                logger.Error("{0C389692-399F-4A1E-9058-251F4CDC6786}");
                return false;
            }

            VTextLine activeLine = this.FindLogicalRow(document, 1);
            if (document.ActiveLine != activeLine)
            {
                logger.Error("{AC7ED387-9205-418A-B4A4-F54757D89795}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 光标左移
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool CUB_CursorBackward()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.ActiveDocument;
            VTCursor cursor = document.Cursor;

            invoker.CUP_CursorPosition(1, 10);
            invoker.CUB_CursorBackward();

            if (cursor.Row != 0 || cursor.Column != 8)
            {
                logger.Error("{9BCFDD25-A5EB-4525-BE84-1F8691894618}");
                return false;
            }

            if (document.ActiveLine != document.FirstLine)
            {
                logger.Error("{3EE2E8F7-644B-4ECC-9F7F-98827FE813E7}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 光标右移
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool CUF_CursorForward()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.ActiveDocument;
            VTCursor cursor = document.Cursor;


            invoker.CUP_CursorPosition(2, 1);
            invoker.CUF_CursorForward();

            if (cursor.Row != 1 || cursor.Column != 1)
            {
                logger.Error("{82EE7106-43DF-40F7-A634-2A073473A163}");
                return false;
            }

            if (document.ActiveLine != document.FirstLine.NextLine)
            {
                logger.Error("{4F32BFBB-15AC-4524-A2C9-A6A7294F7A50}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 光标上移
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool CUU_CursorUp()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.ActiveDocument;
            VTCursor cursor = document.Cursor;

            invoker.CUP_CursorPosition(2, 1);
            invoker.CUU_CursorUp();

            if (cursor.Row != 0 || cursor.Column != 0)
            {
                logger.Error("{E007FC7E-204E-4183-A5DF-6CAEB29F6B01}");
                return false;
            }

            VTextLine activeLine = this.FindLogicalRow(document, 0);
            if (activeLine != document.ActiveLine)
            {
                logger.Error("{2BFF7503-D6FA-4EB2-86FC-C9A33075F00D}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 光标下移
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool CUD_CursorDown()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.ActiveDocument;
            VTCursor cursor = document.Cursor;

            invoker.CUP_CursorPosition(1, 1);
            invoker.CUD_CursorDown();

            if (cursor.Row != 1 || cursor.Column != 0)
            {
                logger.Error("{91FF0537-E95C-484E-86AA-16573CF03E56}");
                return false;
            }

            VTextLine activeLine = this.FindLogicalRow(document, 1);
            if (activeLine != document.ActiveLine)
            {
                logger.Error("{AF867E5F-4526-4B84-9817-F4F182DF912F}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool SGR_SetGraphicsRendition()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.ActiveDocument;
            VTCursor cursor = document.Cursor;

            invoker.SGR();

            return false;
        }

        [UnitTest]
        public bool ED_EraseDisplay()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region ToEnd

            /* 从第2行开始删除到结尾，然后判断内容是否正确 */

            invoker.CUP_CursorPosition(2, 1);
            invoker.ED_EraseDisplay(Terminal.Parsing.VTEraseType.ToEnd);

            List<string> textLines2 = UnitTestHelper.BuildWhitespaceTextLines(row, col);
            textLines2[0] = textLines[0];

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{D9B3DD17-A935-4323-8673-D36662D36C93}");
                return false;
            }

            #endregion

            #region FromBeginning

            /* 光标移动到右上角，删除之前的所有数据，然后判断内容是否正确 */

            invoker.CUP_CursorPosition(1, col);
            invoker.ED_EraseDisplay(Terminal.Parsing.VTEraseType.FromBeginning);

            string line1 = VTUtils.CreatePlainText(document.FirstLine.Characters);
            string line2 = string.Join(string.Empty, Enumerable.Repeat<string>(" ", col));
            if (line1 != line2)
            {
                logger.Error("{79462305-F817-4880-9D39-561287102B69}");
                return false;
            }

            #endregion

            #region All

            /* 光标移动到左上角，重新打印文档，然后删除所有内容，最后比对 */

            invoker.CUP_CursorPosition(1, 1);
            textLines = UnitTestHelper.BuildTextLines(row);
            UnitTestHelper.DrawTextLines(terminal, textLines);
            invoker.ED_EraseDisplay(Terminal.Parsing.VTEraseType.All);

            textLines = UnitTestHelper.BuildWhitespaceTextLines(row, col);
            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("B5AACA29-02DE-49BF-85D9-7F957FFF5922");
                return false;
            }

            #endregion

            return true;
        }

        [UnitTest]
        public bool EL_EraseLine()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region ToEnd

            /* 光标移动到左上角，执行ToEnd，然后比对所有数据 */

            invoker.CUP_CursorPosition(1, 1);
            invoker.EL_EraseLine(Terminal.Parsing.VTEraseType.ToEnd);

            textLines[0] = UnitTestHelper.BuildWhitespaceTextLine(col);
            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{69707744-06DA-48DD-B234-E5121231DD20}");
                return false;
            }

            #endregion

            #region FromBeginning

            /* 光标移动到第二行第10列，执行ToEnd指令，然后比对所有数据 */

            invoker.CUP_CursorPosition(2, 10);
            invoker.EL_EraseLine(Terminal.Parsing.VTEraseType.FromBeginning);

            textLines[1] = textLines[1].Substring(10, col - 10).PadLeft(col, ' ');
            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{9B986000-DFBD-4031-AEE1-BBE2349234C5}");
                return false;
            }

            #endregion

            #region All

            invoker.CUP_CursorPosition(3, 30);
            invoker.EL_EraseLine(Terminal.Parsing.VTEraseType.All);

            textLines[2] = UnitTestHelper.BuildWhitespaceTextLine(col);
            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{0B60EBEA-501A-4DA0-AE1F-C915828D5A83}");
                return false;
            }

            #endregion

            return true;
        }

        [UnitTest]
        public bool DCH_DeleteCharacter()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 移动到左上角，删除3个字符，然后比对 */
            invoker.CUP_CursorPosition(1, 1);
            invoker.DCH_DeleteCharacter(3);

            textLines[0] = textLines[0].Substring(3);

            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{2076DDD6-2EE4-4C68-852D-6E5E144BBB25}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool ICH_InsertCharacter()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 从第2行第5个字符开始插入10个空白字符，然后比对 */
            invoker.CUP_CursorPosition(2, 5);
            invoker.ICH_InsertCharacter(5);

            List<string> textLines2 = UnitTestHelper.BuildTextLines(document);

            textLines[1] = textLines[1].Substring(0, 4) + string.Join("", Enumerable.Repeat(" ", 5)) + textLines[1].Substring(4);
            textLines[1] = textLines[1].Substring(0, col);

            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{2FD3CAFF-091C-4000-A44B-6006E05CC5B4}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool LineFeed()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            List<string> textLines = new List<string>();

            /* 先打印满屏数据，再判断 */

            for (int i = 0; i < row; i++)
            {
                string textLine = UnitTestHelper.BuildTextLineRandom(col);
                textLines.Add(textLine);
                UnitTestHelper.DrawTextLine(terminal, textLine);

                if (i < row - 1)
                {
                    invoker.CRLF();
                }
            }

            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{9793EEF6-D2F6-45F9-8D3F-D0B87EB733B7}");
                return false;
            }

            if (scrollInfo.Value != 0)
            {
                logger.Error("{D3EA136C-2E8D-4C4B-8845-D8EC39AFEABA}");
                return false;
            }

            if (history.Lines != row)
            {
                logger.Error("{DDC62E32-15CE-4F2D-8FB0-6739EF311A71}");
                return false;
            }

            /* 再继续打印10行数据，判断可视区域内容历史内容和滚动条数据 */

            for (int i = 0; i < 10; i++)
            {
                invoker.CRLF();
                string textLine = UnitTestHelper.BuildTextLineRandom(col);
                textLines.Add(textLine);
                UnitTestHelper.DrawTextLine(terminal, textLine);
            }

            // 判断历史数据
            if (!UnitTestHelper.CompareHistory(history, textLines))
            {
                logger.Error("{6094AC7A-BF13-4054-A13D-D5066F9A163B}");
                return false;
            }

            // 判断可视区域数据
            if (!UnitTestHelper.CompareDocument(document, textLines.Skip(10).ToList()))
            {
                logger.Error("{7292BC12-A9EE-4112-BC5C-F63314758B16}");
                return false;
            }

            if (scrollInfo.Value != 10)
            {
                logger.Error("{5E21657A-B911-4463-85C8-B8298927B742}");
                return false;
            }

            if (cursor.Row != row - 1)
            {
                logger.Error("{0CA3B56E-DBA3-413E-BCCA-FFF0A3514B2E}");
                return false;
            }

            if (cursor.PhysicsRow != textLines.Count - 1)
            {
                logger.Error("{B9039CA2-364E-4D0B-A8E2-227D07ECDD79}");
                return false;
            }

            if (!this.LineFeed_Alternate())
            {
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool RI_ReverseLineFeed()
        {
            if (!this.RI_ReverseLineFeed1())
            {
                return false;
            }

            if (!this.RI_ReverseLineFeed2())
            {
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool DECSTBM_SetScrollingRegion()
        {
            if (!this.DECSTBM_SetScrollingRegion_LineFeed())
            {
                return false;
            }

            if (!this.DECSTBM_SetScrollingRegion_RI_ReserveLine())
            {
                return false;
            }

            if (!this.DECSTBM_SetScrollingRegion_SD_ScrollDown())
            {
                return false;
            }

            if (!this.DECSTBM_SetScrollingRegion_SU_ScrollUp())
            {
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool DL_DeleteLine()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;

            invoker.PrintLines(9);

            /* 从第2行开始删除，删除2行，然后比对可视区域内容 */

            invoker.CUP_CursorPosition(2, 1);
            invoker.DL_DeleteLine(2);

            List<string> textLines2 = new List<string>() { "1", "4", "5", "6", "7", "8", "9", "", "" };
            List<string> textLines3 = UnitTestHelper.BuildTextLines(document);

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{ADA8B1B0-D2D4-491F-92A5-85CFCD66C47A}");
                return false;
            }

            /* 比对ActiveLine */
            VTextLine activeLine = document.FindLine(cursor.Row);
            if (document.ActiveLine != activeLine)
            {
                logger.Error("{C4D7CBCD-1149-4F25-8537-A1E939405FF0}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool IL_InsertLine()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTCursor cursor = document.Cursor;

            List<string> textLines = UnitTestHelper.BuildTextLines(9);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 在第2行之前插入2行数据，然后比对 */
            invoker.CUP_CursorPosition(2, 1);
            invoker.IL_InsertLine(2);

            List<string> textLines2 = new List<string>() { "1", "", "", "2", "3", "4", "5", "6", "7" };
            List<string> textLines3 = UnitTestHelper.BuildTextLines(document);

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{29B02C4A-048B-43C1-ACE5-9C6A0737C0B4}");
                return false;
            }

            VTextLine activeLine = document.FindLine(cursor.Row);
            if (document.ActiveLine != activeLine)
            {
                logger.Error("{FDC6B154-1CE6-40F0-813F-1800AEC9493D}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool ECH_EraseCharacters()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 光标移动到第2行5列，然后擦除5个字符，比对 */
            invoker.CUP_CursorPosition(2, 5);
            invoker.ECH_EraseCharacters(5);

            List<string> textLines2 = UnitTestHelper.BuildTextLines(document);

            string str = textLines[1];
            textLines[1] = str.Substring(0, 4) + string.Join("", Enumerable.Repeat(" ", 5)) + str.Substring(9);

            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{CA3CBE88-2B00-47AC-9084-651AFC05FED7}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool SD_ScrollDown()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;

            List<string> textLines = UnitTestHelper.BuildTextLines(9);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 执行SD_ScrollDown，最后比对 */

            invoker.SD_ScrollDown(2);

            List<string> textLines2 = new List<string>() { "", "", "1", "2", "3", "4", "5", "6", "7" };
            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{D48D1480-EAD1-4AE7-BD9C-BD396745839E}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool SU_ScrollUp()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;

            List<string> textLines = UnitTestHelper.BuildTextLines(9);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 执行SD_ScrollUp，最后比对 */

            invoker.SU_ScrollUp(2);

            List<string> textLines2 = new List<string>() { "3", "4", "5", "6", "7", "8", "9", "", "" };
            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{0737A7E6-32B1-4367-9858-56045F894888}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool REP_RepeatCharacter()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;

            invoker.Print('A');
            invoker.REP_RepeatCharacter(3);
            invoker.Print('B');
            invoker.REP_RepeatCharacter(3);

            string textLine1 = VTUtils.CreatePlainText(document.FirstLine.Characters);
            string textLine2 = "AAAABBBB";
            if (textLine1 != textLine2)
            {
                logger.Error("{683D04DC-8960-461D-AC29-3D40DECCF7DF}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool CHA_CursorHorizontalAbsolute()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTCursor cursor = document.Cursor;

            invoker.CHA_CursorHorizontalAbsolute(5);
            if (cursor.Row != 0)
            {
                logger.Error("{7582EB69-FE89-4A5A-B3BF-95880A6BCD8D}");
                return false;
            }

            if (cursor.Column != 4)
            {
                logger.Error("{3CE1028D-A0DE-4CA4-B0ED-ED645AB40F9E}");
                return false;
            }

            return true;
        }

        [UnitTest]
        public bool VPA_VerticalLinePositionAbsolute()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal2(9, 10);
            TerminalInvoker invoker = new TerminalInvoker(terminal);
            VTDocument document = terminal.MainDocument;
            VTCursor cursor = document.Cursor;

            invoker.VPA_VerticalLinePositionAbsolute(5);
            if (cursor.Row != 4)
            {
                logger.Error("{8A6C67F1-51A6-422E-9261-A05BDF67D7F2}");
                return false;
            }

            if (cursor.Column != 0)
            {
                logger.Error("{9DA567C6-3562-4748-ADDD-196C1D3CF736}");
                return false;
            }

            VTextLine activeLine = this.FindLogicalRow(document, 4);
            if (activeLine != document.ActiveLine)
            {
                logger.Error("{DED00A90-CC09-4BDC-B297-5505C817B305}");
                return false;
            }

            return true;
        }

        #endregion
    }
}