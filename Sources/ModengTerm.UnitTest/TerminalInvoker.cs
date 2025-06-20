using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Parsing;
using ModengTerm.Terminal.Renderer;
using ModengTerm.UnitTest.Drawing;
using System.Reflection.Metadata;
using System.Text;

namespace ModengTerm.UnitTest
{
    /// <summary>
    /// 负责原始控制序列
    /// </summary>
    public class TerminalInvoker
    {
        private static readonly byte ESC = 0x1b;

        private VideoTerminal videoTerminal;

        public TerminalInvoker(VideoTerminal videoTerminal)
        {
            this.videoTerminal = videoTerminal;
        }

        private void ProcessCtlseq(string ctlseq)
        {
            byte[] bytes = ctlseq.Select(v => Convert.ToByte(v)).ToArray();
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        /// <summary>
        /// 每行打印一个数字，从1开始
        /// </summary>
        /// <param name="rows">一共要打印多少行</param>
        /// <returns></returns>
        public void PrintLines(int rows)
        {
            List<string> textLines = UnitTestHelper.BuildTextLines(rows);
            UnitTestHelper.DrawTextLines(this.videoTerminal, textLines);
        }

        public void Print(char c)
        {
            this.videoTerminal.ProcessRead(new byte[] { (byte)c }, 1);
        }

        /// <summary>
        /// 光标移动到一个指定的位置
        /// 左上角是1,1
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public void CUP_CursorPosition(int row, int col)
        {
            List<byte> result = new List<byte>();
            result.Add(ESC);
            result.Add((byte)'[');
            result.AddRange(Encoding.ASCII.GetBytes(row.ToString()));
            result.Add((byte)';');
            result.AddRange(Encoding.ASCII.GetBytes(col.ToString()));
            result.Add((byte)'H');

            this.videoTerminal.ProcessRead(result.ToArray(), result.Count);
        }

        /// <summary>
        /// 构造一个无参数的CUP指令
        /// </summary>
        /// <returns></returns>
        public void CUP_CursorPosition()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'H' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标右移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUF_CursorForward()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'C' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标上移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUU_CursorUp()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'A' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标下移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUD_CursorDown()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'B' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标左移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUB_CursorBackward()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'D' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        public void ED_EraseDisplay(VTEraseType eraseType)
        {
            int v = (int)eraseType;
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(v.ToString()));
            bytes.Add((byte)'J');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void EL_EraseLine(VTEraseType eraseType)
        {
            int v = (int)eraseType;
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(v.ToString()));
            bytes.Add((byte)'K');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void DCH_DeleteCharacter(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'P');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void ICH_InsertCharacter(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'@');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void CRLF()
        {
            byte[] bytes = new byte[] { (byte)'\r', (byte)'\n' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        public void LF_FF_VT()
        {
            byte[] bytes = new byte[] { (byte)'\n' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        public void RI_ReverseLineFeed()
        {
            byte[] bytes = new byte[] { ESC, (byte)'M' };
            this.videoTerminal.ProcessRead(bytes, bytes.Length);
        }

        public void DECSTBM_SetScrollingRegion(int topMargin, int bottomMargin)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(topMargin.ToString()));
            bytes.Add((byte)';');
            bytes.AddRange(Encoding.ASCII.GetBytes(bottomMargin.ToString()));
            bytes.Add((byte)'r');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void DL_DeleteLine(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'M');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void IL_InsertLine(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'L');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void ECH_EraseCharacters(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'X');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void SD_ScrollDown(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'T');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void SU_ScrollUp(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'S');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void REP_RepeatCharacter(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'b');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void CHA_CursorHorizontalAbsolute(int col)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(col.ToString()));
            bytes.Add((byte)'G');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void VPA_VerticalLinePositionAbsolute(int row)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(row.ToString()));
            bytes.Add((byte)'d');
            this.videoTerminal.ProcessRead(bytes.ToArray(), bytes.Count);
        }

        public void SGR()
        {
            string ctlseq = "\x1b[48;5;10;38;5;16m";
            this.ProcessCtlseq(ctlseq);
        }

        public void SimulateMouseDown(int rowIndex)
        {
            int row = rowIndex + 1;

            VTDocument document = this.videoTerminal.MainDocument;
            double mouseY = document.Typeface.Height * row - 3;
            FakeGI gi = document.GFactory as FakeGI;
            gi.RaiseMouseDown(new MouseData() { ClickCount = 1, X = 10, Y = mouseY });
        }

        public void ASB_AlternateScreenBuffer(bool enable) 
        {
            char state = enable ? 'h' : 'l';

            string ctlseq = "\x1b[?1049" + state;
            this.ProcessCtlseq(ctlseq);
        }
    }
}
