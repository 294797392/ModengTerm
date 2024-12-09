using ModengTerm.Terminal;
using ModengTerm.Terminal.Parsing;
using ModengTerm.Terminal.Renderer;
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
            this.videoTerminal.ProcessData(bytes, bytes.Length);
        }


        public void Print(char c)
        {
            this.videoTerminal.ProcessData(new byte[] { (byte)c }, 1);
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

            this.videoTerminal.ProcessData(result.ToArray(), result.Count);
        }

        /// <summary>
        /// 构造一个无参数的CUP指令
        /// </summary>
        /// <returns></returns>
        public void CUP_CursorPosition()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'H' };
            this.videoTerminal.ProcessData(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标右移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUF_CursorForward()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'C' };
            this.videoTerminal.ProcessData(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标上移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUU_CursorUp()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'A' };
            this.videoTerminal.ProcessData(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标下移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUD_CursorDown()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'B' };
            this.videoTerminal.ProcessData(bytes, bytes.Length);
        }

        /// <summary>
        /// 光标左移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public void CUB_CursorBackward()
        {
            byte[] bytes = new byte[] { ESC, (byte)'[', (byte)'D' };
            this.videoTerminal.ProcessData(bytes, bytes.Length);
        }

        public void ED_EraseDisplay(VTEraseType eraseType)
        {
            int v = (int)eraseType;
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(v.ToString()));
            bytes.Add((byte)'J');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count); 
        }

        public void EL_EraseLine(VTEraseType eraseType)
        {
            int v = (int)eraseType;
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(v.ToString()));
            bytes.Add((byte)'K');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void DCH_DeleteCharacter(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'P');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void ICH_InsertCharacter(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'@');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void CRLF()
        {
            byte[] bytes = new byte[] { (byte)'\r', (byte)'\n' };
            this.videoTerminal.ProcessData(bytes, bytes.Length);
        }

        public void RI_ReverseLineFeed()
        {
            byte[] bytes = new byte[] { ESC, (byte)'M' };
            this.videoTerminal.ProcessData(bytes, bytes.Length);
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
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void DL_DeleteLine(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'M');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void IL_InsertLine(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'L');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void ECH_EraseCharacters(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'X');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void SD_ScrollDown(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'T');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void SU_ScrollUp(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'S');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void REP_RepeatCharacter(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'b');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void CHA_CursorHorizontalAbsolute(int col) 
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(col.ToString()));
            bytes.Add((byte)'G');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void VPA_VerticalLinePositionAbsolute(int row) 
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(row.ToString()));
            bytes.Add((byte)'d');
            this.videoTerminal.ProcessData(bytes.ToArray(), bytes.Count);
        }

        public void SGR()
        {
            string ctlseq = "\x1b[48;5;10;38;5;16m";
            this.ProcessCtlseq(ctlseq);
        }
    }
}
