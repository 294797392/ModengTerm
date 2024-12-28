using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using ModengTerm.UnitTest.Drawing;
using System.Text;
using System.Windows.Media.Media3D;
using XTerminal.Base.Definitions;

namespace ModengTerm.UnitTest
{
    public static class UnitTestHelper
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("UnitTestHelper");

        #region VTDocument

        public static VTDocument CreateVTDocument()
        {
            VTDocumentOptions options = new VTDocumentOptions()
            {
                ScrollDelta = 1,
                Name = "TestDocument",
                RollbackMax = 1000,
                ViewportRow = 80,
                ViewportColumn = 24,
                Typeface = new VTypeface()
                {
                    Width = 20,
                    Height = 20
                },
                GraphicsInterface = new FakeGI()
            };

            VTDocument document = new VTDocument(options);
            document.Initialize();
            return document;
        }

        /// <summary>
        /// 只比对文档，不比对历史记录
        /// </summary>
        /// <param name="document"></param>
        /// <param name="textLines"></param>
        /// <returns></returns>
        public static bool CompareDocument2(VTDocument document, List<string> textLines)
        {
            VTextLine current = document.FirstLine;

            foreach (string textLine in textLines)
            {
                string line1 = VTDocUtils.CreatePlainText(current.Characters);
                string line2 = textLine;

                if (line1 != line2)
                {
                    logger.Error("{3EB3D6EF-029D-42C7-8C17-E18F86EFEDAC}");
                    return false;
                }

                current = current.NextLine;
            }

            return true;
        }


        /// <summary>
        /// 比对文档和历史记录
        /// </summary>
        /// <param name="document"></param>
        /// <param name="textLines"></param>
        /// <returns></returns>
        public static bool CompareDocument(VTDocument document, List<string> textLines)
        {
            VTextLine current = document.FirstLine;

            foreach (string textLine in textLines)
            {
                string line1 = VTDocUtils.CreatePlainText(current.Characters);
                string line2 = textLine;

                if (line1 != line2)
                {
                    logger.Error("CompareDocument, {59EAD176-C44C-40A6-AC2C-5503F224410D}");
                    return false;
                }

                current = current.NextLine;
            }

            // 如果还有剩下的，都必须是空
            while (current != null)
            {
                string line = VTDocUtils.CreatePlainText(current.Characters);
                if (!string.IsNullOrEmpty(line))
                {
                    logger.Error("CompareDocument, {7B8AC313-7892-4561-AF43-B17BD0838E47}");
                    return false;
                }

                current = current.NextLine;
            }

            int left = textLines.Count;

            // 比对可视区域的历史记录
            VTHistory history = document.History;
            current = document.FirstLine;
            while (current != null && left > 0)
            {
                int physicsRow = current.GetPhysicsRow();

                VTHistoryLine historyLine;
                if (!history.TryGetHistory(physicsRow, out historyLine))
                {
                    logger.Error("CompareDocument, {1729E488-0A3A-499A-90C9-9C08572541AD}");
                    return false;
                }

                if (historyLine.Characters != current.Characters)
                {
                    logger.Error("CompareDocument, {E93629F2-4AAF-4511-AFF5-FEAD3F1C4907}");
                    return false;
                }

                current = current.NextLine;
                left--;
            }

            return true;
        }

        public static bool CompareHistory(VTHistory history, List<string> textLines)
        {
            if (history.Lines != textLines.Count)
            {
                logger.ErrorFormat("行数不一致");
                return false;
            }

            for (int i = 0; i < textLines.Count; i++)
            {
                VTHistoryLine historyLine;
                if (!history.TryGetHistory(i, out historyLine))
                {
                    logger.ErrorFormat("查找历史记录失败, 物理行号 = {0}", i);
                    return false;
                }

                string textLine1 = VTDocUtils.CreatePlainText(historyLine.Characters);
                string textLine2 = textLines[i];

                if (textLine1 != textLine2)
                {
                    logger.ErrorFormat("历史记录内容不一致, 物理行号 = {0}", i);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 文档的可视区域和历史记录做比对
        /// </summary>
        /// <param name="history">要比对的历史记录</param>
        /// <param name="startHistoryRow">从历史记录的第几行开始比对</param>
        /// <param name="startLine">要比对的第一行VTextLine</param>
        /// <param name="compareRows">最多比对多少行</param>
        /// <returns></returns>
        public static bool CompareHistory(VTHistory history, int startHistoryRow, VTextLine startLine, int compareRows)
        {
            int leftRows = compareRows;
            int historyRow = startHistoryRow;
            VTextLine current = startLine;

            while (leftRows > 0) 
            {
                VTHistoryLine historyLine;
                if (!history.TryGetHistory(historyRow, out historyLine)) 
                {
                    logger.Error("{F6AB3D10-DF14-42FB-A8D1-F0AA04463C26}");
                    return false;
                }

                if (historyLine != current.History)
                {
                    logger.Error("{F1022064-A12D-42CC-983C-1C499916AFE2}");
                    return false;
                }

                current = current.NextLine;
                historyRow++;
                leftRows--;
            }

            return true;
        }

        public static bool CompareList(List<string> textLines1, List<string> textLines2)
        {
            if (textLines1.Count != textLines2.Count)
            {
                return false;
            }

            for (int i = 0; i < textLines1.Count; i++)
            {
                string textLine1 = textLines1[i];
                string textLine2 = textLines2[i];

                if (textLines1 != textLines2)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region VideoTerminal

        private static int seed = 0;

        public static XTermSession CreateSession(int row, int col)
        {
            MTermManifest manifest = JSONHelper.File2Object<MTermManifest>("app.json");

            // 重写测试使用的Session参数
            XTermSession session = manifest.DefaultSession;
            session.SetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK, 1000);
            session.SetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE, TerminalSizeModeEnum.Fixed);
            session.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, row);
            session.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, col);

            return session;
        }

        public static VideoTerminal CreateVideoTerminal3(XTermSession session)
        {
            VTOptions options = new VTOptions()
            {
                Width = 0,
                Height = 0,
                Session = session,
                AlternateDocument = new FakeGI(),
                MainDocument = new FakeGI()
            };

            VideoTerminal terminal = new VideoTerminal();
            terminal.Initialize(options);

            return terminal;
        }

        /// <summary>
        /// 指定row和col创建一个终端
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static VideoTerminal CreateVideoTerminal2(int row, int col)
        {
            XTermSession session = CreateSession(row, col);
            return CreateVideoTerminal3(session);
        }

        public static VideoTerminal CreateVideoTerminal(int width, int height)
        {
            MTermManifest manifest = JSONHelper.File2Object<MTermManifest>("app.json");

            // 重写测试使用的Session参数
            XTermSession session = manifest.DefaultSession;
            session.SetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK, 1000);

            VTOptions options = new VTOptions()
            {
                Width = width,
                Height = height,
                Session = session,
                AlternateDocument = new FakeGI(),
                MainDocument = new FakeGI()
            };

            VideoTerminal terminal = new VideoTerminal();
            terminal.Initialize(options);
            return terminal;
        }

        public static VideoTerminal CreateVideoTerminal()
        {
            return CreateVideoTerminal(800, 600);
        }

        /// <summary>
        /// 把textLines转换成原始控制序列交给终端处理
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="textLines"></param>
        public static void DrawTextLines(VideoTerminal terminal, List<string> textLines)
        {
            // 原始的控制序列
            List<byte> rawData = new List<byte>();

            foreach (string textLine in textLines)
            {
                rawData.AddRange(Encoding.UTF8.GetBytes(textLine));
                rawData.Add((byte)'\r');
                rawData.Add((byte)'\n');
            }

            terminal.ProcessRead(rawData.ToArray(), rawData.Count - 2); // 去掉最后的CRLF
        }

        public static void DrawTextLine(VideoTerminal terminal, string textLine)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(textLine);
            terminal.ProcessRead(rawData, rawData.Length);
        }

        /// <summary>
        /// 生成一行随机字符串文本
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static string BuildTextLineRandom(int cols)
        {
            string textLine = string.Empty;

            for (int i = 0; i < cols; i++)
            {
                Random random = new Random(seed++);
                if (seed == int.MaxValue)
                {
                    seed = 0;
                }
                char c = (char)random.Next(33, 126);
                textLine += c;
            }

            return textLine;
        }

        /// <summary>
        /// 生成一行1,2,3,4...cols的文本
        /// </summary>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static string BuildTextLine(int cols)
        {
            string textLine = string.Empty;

            for (int i = 1; i <= cols; i++)
            {
                textLine += i.ToString();
            }

            return textLine;
        }

        /// <summary>
        /// 生成1，2，3，4，5的数据
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static List<string> BuildTextLines(int rows)
        {
            List<string> textLines = new List<string>();

            for (int i = 1; i <= rows; i++)
            {
                textLines.Add(i.ToString());
            }

            return textLines;
        }

        public static List<string> BuildTextLines(int rows, int cols)
        {
            List<string> textLines = new List<string>();

            for (int i = 0; i < rows; i++)
            {
                string textLine = BuildTextLineRandom(cols);

                textLines.Add(textLine);
            }

            return textLines;
        }

        public static List<string> BuildWhitespaceTextLines(int rows, int cols)
        {
            List<string> textLines = new List<string>();

            string textLine1 = string.Join("", Enumerable.Repeat(" ", cols));

            return Enumerable.Repeat(textLine1, rows).ToList();
        }

        public static List<string> BuildTextLines(VTDocument document)
        {
            List<string> textLines = new List<string>();

            VTextLine textLine = document.FirstLine;

            while (textLine != null)
            {
                string line = VTDocUtils.CreatePlainText(textLine.Characters);

                textLines.Add(line);

                textLine = textLine.NextLine;
            }

            return textLines;
        }

        public static string BuildWhitespaceTextLine(int cols)
        {
            return string.Join(string.Empty, Enumerable.Repeat<string>(" ", cols));
        }

        #endregion
    }
}