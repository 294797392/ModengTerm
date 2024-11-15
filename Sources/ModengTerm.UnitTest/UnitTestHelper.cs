using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using ModengTerm.UnitTest.Drawing;
using System.Text;

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
                Controller = new FakeDocument()
            };

            VTDocument document = new VTDocument(options);
            document.Initialize();
            return document;
        }

        public static bool DocumentCompare(VTDocument document, List<string> textLines)
        {
            VTextLine current = document.FirstLine;

            foreach (string textLine in textLines)
            {
                string line1 = VTUtils.CreatePlainText(current.Characters);
                string line2 = textLine;

                if (line1 != line2)
                {
                    return false;
                }

                current = current.NextLine;
            }

            return true;
        }

        #endregion

        #region VideoTerminal

        private static int seed = 0;

        public static string GenerateRandomLine(int cols) 
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

        public static VideoTerminal CreateVideoTerminal()
        {
            MTermManifest manifest = JSONHelper.File2Object<MTermManifest>("app.json");

            // 重写测试使用的Session参数
            XTermSession session = manifest.DefaultSession;
            session.SetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK, 1000);

            VTOptions options = new VTOptions()
            {
                Width = 800,
                Height = 600,
                Session = session,
                AlternateDocument = new FakeDocument(),
                MainDocument = new FakeDocument()
            };

            VideoTerminal terminal = new VideoTerminal();
            terminal.Initialize(options);
            return terminal;
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

            terminal.ProcessData(rawData.ToArray(), rawData.Count - 2); // 去掉最后的CRLF
        }

        public static void VideoTerminalRender(VideoTerminal terminal, string textLine)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(textLine);
            terminal.ProcessData(rawData, rawData.Length);
        }

        public static bool VideoTerminalCompareHistory(VTHistory history, List<string> textLines)
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

                string textLine1 = VTUtils.CreatePlainText(historyLine.Characters);
                string textLine2 = textLines[i];

                if (textLine1 != textLine2)
                {
                    logger.ErrorFormat("历史记录内容不一致, 物理行号 = {0}", i);
                    return false;
                }
            }

            return true;
        }

        public static List<string> BuildTextLines(int rows)
        {
            List<string> textLines = new List<string>();

            for (int i = 0; i < rows; i++)
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
                string textLine = GenerateRandomLine(cols);

                textLines.Add(textLine);
            }

            return textLines;
        }

        #endregion
    }
}