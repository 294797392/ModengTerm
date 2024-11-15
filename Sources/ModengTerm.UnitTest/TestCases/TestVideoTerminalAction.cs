using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;

namespace ModengTerm.UnitTest.TestCases
{
    /// <summary>
    /// 模拟SshServer，手动生成原始的控制序列让终端处理，然后判断终端渲染之后的数据是否正确
    /// </summary>
    public class TestVideoTerminalAction
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TestVideoTerminalEngine");

        #region 针对于每个指令做单元测试

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

                textLines[i] = UnitTestHelper.GenerateRandomLine(col);
                UnitTestHelper.VideoTerminalRender(terminal, textLines[i]);
                if (!UnitTestHelper.DocumentCompare(document, textLines))
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

        /// <summary>
        /// 测试SGR指令
        /// </summary>
        /// <returns></returns>
        [UnitTest]
        public bool SGR_SetGraphicsRendition() 
        {
            return false;
        }

        #endregion
    }
}
