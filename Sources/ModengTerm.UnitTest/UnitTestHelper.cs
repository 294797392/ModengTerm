﻿using DotNEToolkit;
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
                Controller = new FakeDocument()
            };

            VTDocument document = new VTDocument(options);
            document.Initialize();
            return document;
        }

        public static bool CompareDocument(VTDocument document, List<string> textLines)
        {
            VTextLine current = document.FirstLine;

            foreach (string textLine in textLines)
            {
                string line1 = VTUtils.CreatePlainText(current.Characters);
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
                string line = VTUtils.CreatePlainText(current.Characters);
                if (!string.IsNullOrEmpty(line))
                {
                    logger.Error("CompareDocument, {7B8AC313-7892-4561-AF43-B17BD0838E47}");
                    return false;
                }

                current = current.NextLine;
            }

            // 比对可视区域的历史记录
            VTHistory history = document.History;
            current = document.FirstLine;
            while (current != null)
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

        /// <summary>
        /// 指定row和col创建一个终端
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static VideoTerminal CreateVideoTerminal2(int row, int col) 
        {
            MTermManifest manifest = JSONHelper.File2Object<MTermManifest>("app.json");

            // 重写测试使用的Session参数
            XTermSession session = manifest.DefaultSession;
            session.SetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK, 1000);
            session.SetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE, TerminalSizeModeEnum.Fixed);
            session.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, row);
            session.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, col);

            VTOptions options = new VTOptions()
            {
                Width = 0,
                Height = 0,
                Session = session,
                AlternateDocument = new FakeDocument(),
                MainDocument = new FakeDocument()
            };

            VideoTerminal terminal = new VideoTerminal();
            terminal.Initialize(options);
            return terminal;
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
                AlternateDocument = new FakeDocument(),
                MainDocument = new FakeDocument()
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

            terminal.ProcessData(rawData.ToArray(), rawData.Count - 2); // 去掉最后的CRLF
        }

        public static void DrawTextLine(VideoTerminal terminal, string textLine)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(textLine);
            terminal.ProcessData(rawData, rawData.Length);
        }

        public static string BuildTextLine(int cols)
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
                string textLine = BuildTextLine(cols);

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
                string line = VTUtils.CreatePlainText(textLine.Characters);

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