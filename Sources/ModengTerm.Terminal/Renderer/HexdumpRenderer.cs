using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;

namespace ModengTerm.Terminal.Renderer
{
    /// <summary>
    /// 将收到的数据渲染为hexdump形式的格式
    /// </summary>
    public class HexdumpRenderer : RendererBase
    {
        #region 实例变量

        private VTDocument document;
        private VTextAttributeState writeTextAttr;
        private VTextAttributeState readTextAttr;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public HexdumpRenderer(VideoTerminal vt) :
            base(vt)
        {
            this.document = vt.MainDocument;
        }

        #endregion

        #region VTermRenderer

        public override void Initialize()
        {
            string sendColor = this.session.GetOption<string>(OptionKeyEnum.TERM_ADVANCE_SEND_COLOR, OptionDefaultValues.TERM_ADVANCE_SEND_COLOR);
            this.writeTextAttr = this.CreateForegroundAttribute(sendColor);
            string recvColor = this.session.GetOption<string>(OptionKeyEnum.TERM_ADVANCE_RECV_COLOR, OptionDefaultValues.TERM_ADVANCE_RECV_COLOR);
            this.readTextAttr = this.CreateForegroundAttribute(recvColor);
        }

        public override void Release()
        {
        }

        public override void RenderRead(byte[] bytes, int length)
        {
            this.RenderHex(bytes, length, this.readTextAttr);
        }

        public override void RenderWrite(byte[] bytes)
        {
            this.RenderHex(bytes, bytes.Length, this.writeTextAttr);
        }

        #endregion

        #region 实例方法

        private void RenderHex(byte[] bytes, int length, VTextAttributeState textAttr) 
        {
            VTDocument document = this.document;
            int viewportColumn = document.ViewportColumn;
            int viewportRow = document.ViewportRow;

            for (int i = 0; i < length; i++)
            {
                byte value = bytes[i];

                int cursorRow = document.Cursor.Row;
                int cursorCol = document.Cursor.Column;

                // 要在第几列显示
                int printColumn = 0;

                // 要在第几行显示
                int printRow = 0;

                // 计算剩余可以显示多少列
                VTextLine activeLine = document.ActiveLine;
                int leftCols = viewportColumn - activeLine.Columns;

                // 光标所在行的剩余列数至少得有3个字符的位置才能显示一个十六进制数（包含一个空字符)
                if (leftCols <= 3)
                {
                    printRow = cursorRow == viewportRow - 1 ? cursorRow : cursorRow + 1;
                    printColumn = 0;
                    videoTerminal.LineFeed();
                }
                else
                {
                    printRow = cursorRow;
                    printColumn = cursorCol;
                }

                string hexstr = value.ToString("X2");

                document.SetCursorLogical(printRow, printColumn);
                VTCharacter character1 = VTCharacter.CreateEmpty();
                document.PrintCharacter(character1);

                document.SetCursorLogical(printRow, printColumn + 1);
                VTCharacter character2 = VTCharacter.Create(hexstr[0], 1, textAttr);
                document.PrintCharacter(character2);

                document.SetCursorLogical(printRow, printColumn + 2);
                VTCharacter character3 = VTCharacter.Create(hexstr[1], 1, textAttr);
                document.PrintCharacter(character3);

                document.SetCursorLogical(printRow, printColumn + 3);
            }
        }

        #endregion
    }
}
