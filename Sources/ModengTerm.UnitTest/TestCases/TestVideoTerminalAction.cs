using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using System.Windows.Documents.DocumentStructures;

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;
            int oldScrollValue = scrollInfo.Value;
            int oldScrollMax = scrollInfo.Maximum;

            List<string> textLines = UnitTestHelper.BuildTextLines(row);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region LineFeed

            /* 先设置滚动边距，然后在最后一行滚动，最后比对 */

            /* marginTop = 1, marginBottom = 2 */

            buffer = ControlSequenceGenerator.DECSTBM_SetScrollingRegion(2, row - 2);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.CUP_CursorPosition(7, 0);
            terminal.ProcessData(buffer, buffer.Length);
            // 在滚动区域内最后一行执行LineFeed动作，此时滚动区域内最后一行应该为空
            buffer = ControlSequenceGenerator.CRLF();
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;
            int oldScrollValue = scrollInfo.Value;
            int oldScrollMax = scrollInfo.Maximum;

            List<string> textLines = UnitTestHelper.BuildTextLines(row);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region RI_ReserveLine

            /* marginTop = 1, marginBottom = 2 */

            buffer = ControlSequenceGenerator.DECSTBM_SetScrollingRegion(2, 7);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.CUP_CursorPosition(2, 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.RI_ReverseLineFeed();
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            byte[] buffer = new byte[0];

            List<string> textLines = UnitTestHelper.BuildTextLines(5);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 先设置滚动边距，然后执行SD_ScrollDown，最后比对 */

            /* marginTop = 1, marginBottom = 1 */
            buffer = ControlSequenceGenerator.DECSTBM_SetScrollingRegion(2, 5 - 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.SD_ScrollDown(2);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            byte[] buffer = new byte[0];

            List<string> textLines = UnitTestHelper.BuildTextLines(5);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 先设置滚动边距，然后执行SD_ScrollUp，最后比对 */

            /* marginTop = 1, marginBottom = 1 */
            buffer = ControlSequenceGenerator.DECSTBM_SetScrollingRegion(2, 5 - 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.SU_ScrollUp(2);
            terminal.ProcessData(buffer, buffer.Length);

            List<string> textLines2 = new List<string>() { "1", "4", "", "", "5" };
            List<string> textLines3 = UnitTestHelper.BuildTextLines(document);

            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{4BF1251C-9743-45C7-AC2A-FBE8EF35970F}");
                return false;
            }

            return true;
        }

        #region 针对于每个指令做单元测试

        [UnitTest]
        public bool Print()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            VTDocument mainDocument = terminal.MainDocument;
            VTScrollInfo scrollInfo = mainDocument.Scrollbar;
            int row = mainDocument.ViewportRow;
            int col = mainDocument.ViewportColumn;

            #region 直接打印到第5列

            /* 当前行里的VTCharacters是空的，光标直接移动到第5列去打印一个字符A */

            byte[] bytes = ControlSequenceGenerator.CUP_CursorPosition(1, 5);
            terminal.ProcessData(bytes, bytes.Length);
            bytes = new byte[] { (byte)'A' };
            terminal.ProcessData(bytes, bytes.Length);
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
        /// 测试移动光标之后光标数据是否正确
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool CursorMovement()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region 定位测试

            // 移动到左上角
            for (int i = 0; i < row; i++)
            {
                // 光标移动到每一行的开头
                buffer = ControlSequenceGenerator.CUP_CursorPosition(i + 1, 1);
                terminal.ProcessData(buffer, buffer.Length);

                if (cursor.Row != i || cursor.Column != 0)
                {
                    logger.ErrorFormat("C6855363-EBF8-4DAB-B46D-6FF511C18526");
                    return false;
                }

                textLines[i] = UnitTestHelper.BuildTextLine(col);
                UnitTestHelper.DrawTextLine(terminal, textLines[i]);
                if (!UnitTestHelper.CompareDocument(document, textLines))
                {
                    logger.ErrorFormat("30C0E8F1-CF91-41AA-AE57-2922485A98AD");
                    return false;
                }
            }

            #endregion

            #region 左移测试

            // 光标回到第一行最右边
            buffer = ControlSequenceGenerator.CUP_CursorPosition(1, col);
            terminal.ProcessData(buffer, buffer.Length);

            // 左移到最左边
            for (int i = 0; i < col; i++)
            {
                buffer = ControlSequenceGenerator.CUB_CursorBackward();
                terminal.ProcessData(buffer, buffer.Length);
            }

            // 判断移动后的光标位置
            if (cursor.Row != 0 || cursor.Column != 0)
            {
                logger.ErrorFormat("D9055CAA-FEE9-4A83-9192-7DF44FBBB66A");
                return false;
            }

            // 该行打印相同的字符A
            string textLine1 = string.Empty;
            buffer = new byte[] { (byte)'A' };
            for (int i = 0; i < col; i++)
            {
                textLine1 += 'A';
                terminal.ProcessData(buffer, buffer.Length);
            }

            textLines[0] = textLine1;

            // 判断打印之后的字符
            string textLine2 = VTUtils.CreatePlainText(document.FirstLine.Characters);
            if (textLine1 != textLine2)
            {
                logger.ErrorFormat("48421434-48F8-4945-8E9A-16A61A8450FF");
                return false;
            }

            #endregion

            #region 右移测试

            /* 先把光标移动到左上方，然后使用CUF指令移动到最后一列，每次执行完CUF判断光标的位置 */

            // 光标移动到左上方
            buffer = ControlSequenceGenerator.CUP_CursorPosition(1, 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.CUF_CursorForward();

            // 一直往右移直到移动到最右边
            for (int i = 0; i < col - 1; i++)
            {
                terminal.ProcessData(buffer, buffer.Length);

                if (cursor.Row != 0 || cursor.Column != i + 1)
                {
                    logger.ErrorFormat("F5B5595F-5F27-47F4-A71B-156FA75E0DDB");
                    return false;
                }
            }

            #endregion

            #region 上移测试

            /* 先把光标移动到右下方，然后执行5次CUU指令，每次执行完之后判断光标的位置和ActiveLine */

            // 光标移动到右下角
            buffer = ControlSequenceGenerator.CUP_CursorPosition(row, col);
            terminal.ProcessData(buffer, buffer.Length);
            if (cursor.Row != row - 1 || cursor.Column != col - 1)
            {
                logger.ErrorFormat("D74BE3D3-4B08-4A6F-84B5-14B41799B93B");
                return false;
            }

            // 往上移5行
            VTextLine currentLine = document.LastLine;
            buffer = ControlSequenceGenerator.CUU_CursorUp();
            for (int i = 0; i < 5; i++)
            {
                terminal.ProcessData(buffer, buffer.Length);
                // 判断ActiveLine
                currentLine = currentLine.PreviousLine;
                if (document.ActiveLine != currentLine)
                {
                    logger.ErrorFormat("DBEF83F7-C535-422D-BD28-F542811059C3");
                    return false;
                }

                if (cursor.Row != row - i - 2 || cursor.Column != col - 1)
                {
                    logger.ErrorFormat("BA1FAA78-A0E6-4C31-A1E6-DAA080EE3FE1");
                    return false;
                }
            }

            #endregion

            #region 下移测试

            /* 先把光标移动到左上方，然后执行5次CUD指令，每次执行完之后判断光标的位置和ActiveLine */

            // 光标移动到左上角
            buffer = ControlSequenceGenerator.CUP_CursorPosition(1, 1);
            terminal.ProcessData(buffer, buffer.Length);
            if (cursor.Row != 0 || cursor.Column != 0)
            {
                logger.ErrorFormat("B7EB3D01-38E3-4165-BE53-1ECCE4697850");
                return false;
            }

            // 往上移5行
            currentLine = document.FirstLine;
            buffer = ControlSequenceGenerator.CUD_CursorDown();
            for (int i = 0; i < 5; i++)
            {
                terminal.ProcessData(buffer, buffer.Length);
                // 判断ActiveLine
                currentLine = currentLine.NextLine;
                if (document.ActiveLine != currentLine)
                {
                    logger.ErrorFormat("B7EB3D01-38E3-4165-BE53-1ECCE4697850");
                    return false;
                }

                if (cursor.Row != i + 1 || cursor.Column != 0)
                {
                    logger.ErrorFormat("3DCF1118-26CF-4226-8D3E-2493DCBE728B");
                    return false;
                }
            }

            #endregion

            // TODO：加一些边缘测试（比如光标移动到了一个无法到达的地方）

            return true;
        }

        //[UnitTest]
        //public bool SGR_SetGraphicsRendition()
        //{
        //    return false;
        //}

        [UnitTest]
        public bool ED_EraseDisplay()
        {
            VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region ToEnd

            /* 从第2行开始删除到结尾，然后判断内容是否正确 */

            buffer = ControlSequenceGenerator.CUP_CursorPosition(2, 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.ED_EraseDisplay(Terminal.Parsing.VTEraseType.ToEnd);
            terminal.ProcessData(buffer, buffer.Length);

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

            buffer = ControlSequenceGenerator.CUP_CursorPosition(1, col);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.ED_EraseDisplay(Terminal.Parsing.VTEraseType.FromBeginning);
            terminal.ProcessData(buffer, buffer.Length);

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

            buffer = ControlSequenceGenerator.CUP_CursorPosition(1, 1);
            terminal.ProcessData(buffer, buffer.Length);
            textLines = UnitTestHelper.BuildTextLines(row);
            UnitTestHelper.DrawTextLines(terminal, textLines);
            buffer = ControlSequenceGenerator.ED_EraseDisplay(Terminal.Parsing.VTEraseType.All);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            #region ToEnd

            /* 光标移动到左上角，执行ToEnd，然后比对所有数据 */

            buffer = ControlSequenceGenerator.CUP_CursorPosition(1, 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.EL_EraseLine(Terminal.Parsing.VTEraseType.ToEnd);
            terminal.ProcessData(buffer, buffer.Length);

            textLines[0] = UnitTestHelper.BuildWhitespaceTextLine(col);
            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{69707744-06DA-48DD-B234-E5121231DD20}");
                return false;
            }

            #endregion

            #region FromBeginning

            /* 光标移动到第二行第10列，执行ToEnd指令，然后比对所有数据 */

            buffer = ControlSequenceGenerator.CUP_CursorPosition(2, 10);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.EL_EraseLine(Terminal.Parsing.VTEraseType.FromBeginning);
            terminal.ProcessData(buffer, buffer.Length);

            textLines[1] = textLines[1].Substring(10, col - 10).PadLeft(col, ' ');
            if (!UnitTestHelper.CompareDocument(document, textLines))
            {
                logger.Error("{9B986000-DFBD-4031-AEE1-BBE2349234C5}");
                return false;
            }

            #endregion

            #region All

            buffer = ControlSequenceGenerator.CUP_CursorPosition(3, 30);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.EL_EraseLine(Terminal.Parsing.VTEraseType.All);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 移动到左上角，删除3个字符，然后比对 */
            buffer = ControlSequenceGenerator.CUP_CursorPosition(1, 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.DCH_DeleteCharacter(3);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 从第2行第5个字符开始插入10个空白字符，然后比对 */
            buffer = ControlSequenceGenerator.CUP_CursorPosition(2, 5);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.ICH_InsertCharacter(5);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;
            List<string> textLines = new List<string>();

            /* 先打印满屏数据，再判断 */

            for (int i = 0; i < row; i++)
            {
                string textLine = UnitTestHelper.BuildTextLine(col);
                textLines.Add(textLine);
                UnitTestHelper.DrawTextLine(terminal, textLine);

                if (i < row - 1)
                {
                    buffer = ControlSequenceGenerator.CRLF();
                    terminal.ProcessData(buffer, buffer.Length);
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
                buffer = ControlSequenceGenerator.CRLF();
                terminal.ProcessData(buffer, buffer.Length);
                string textLine = UnitTestHelper.BuildTextLine(col);
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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(9);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 从第2行开始删除，删除2行，然后比对可视区域内容 */

            buffer = ControlSequenceGenerator.CUP_CursorPosition(2, 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.DL_DeleteLine(2);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTCursor cursor = document.Cursor;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(9);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 在第2行之前插入2行数据，然后比对 */
            buffer = ControlSequenceGenerator.CUP_CursorPosition(2, 1);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.IL_InsertLine(2);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = terminal.ScrollInfo;
            VTCursor cursor = document.Cursor;
            int row = document.ViewportRow;
            int col = document.ViewportColumn;
            byte[] buffer = null;

            List<string> textLines = UnitTestHelper.BuildTextLines(row, col);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 光标移动到第2行5列，然后擦除5个字符，比对 */
            buffer = ControlSequenceGenerator.CUP_CursorPosition(2, 5);
            terminal.ProcessData(buffer, buffer.Length);
            buffer = ControlSequenceGenerator.ECH_EraseCharacters(5);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            byte[] buffer = new byte[0];

            List<string> textLines = UnitTestHelper.BuildTextLines(9);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 执行SD_ScrollDown，最后比对 */

            buffer = ControlSequenceGenerator.SD_ScrollDown(2);
            terminal.ProcessData(buffer, buffer.Length);

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
            VTDocument document = terminal.MainDocument;
            VTHistory history = document.History;
            byte[] buffer = new byte[0];

            List<string> textLines = UnitTestHelper.BuildTextLines(9);
            UnitTestHelper.DrawTextLines(terminal, textLines);

            /* 执行SD_ScrollUp，最后比对 */

            buffer = ControlSequenceGenerator.SU_ScrollUp(2);
            terminal.ProcessData(buffer, buffer.Length);

            List<string> textLines2 = new List<string>() { "3", "4", "5", "6", "7", "8", "9", "", "" };
            if (!UnitTestHelper.CompareDocument(document, textLines2))
            {
                logger.Error("{0737A7E6-32B1-4367-9858-56045F894888}");
                return false;
            }

            return true;
        }

        #endregion
    }
}