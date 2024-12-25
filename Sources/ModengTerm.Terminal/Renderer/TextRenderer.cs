using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using System.Text;

namespace ModengTerm.Terminal.Renderer
{
    /// <summary>
    /// 将收到的数据按照可打印字符渲染（包含中文）
    /// 会处理换行逻辑
    /// </summary>
    public class TextRenderer : VTermRenderer
    {
        #region 实例变量

        private VTDocument document;
        private VTextAttributeState writeTextAttr;
        private VTextAttributeState readTextAttr;

        #endregion

        #region 构造方法

        public TextRenderer(VideoTerminal vt) : 
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
            this.RenderText(bytes, length, this.readTextAttr);
        }

        public override void RenderWrite(byte[] bytes)
        {
            this.RenderText(bytes, bytes.Length, this.writeTextAttr);
        }

        #endregion

        #region 实例方法

        private void RenderText(byte[] bytes, int length, VTextAttributeState textAttr) 
        {
            VTDocument document = this.document;
            int viewportColumn = document.ViewportColumn;
            int viewportRow = document.ViewportRow;

            string text = Encoding.UTF8.GetString(bytes, 0, length);

            foreach (char ch in text)
            {
                int cursorRow = this.document.Cursor.Row;
                int cursorCol = this.document.Cursor.Column;

                // 要在哪一行打印
                int printRow = 0, printColumn = 0;

                // 计算剩余可以显示多少列
                VTextLine activeLine = document.ActiveLine;
                int leftCols = viewportColumn - activeLine.Columns;

                int column = 0;

                if (ch >= 32 && ch <= 127)
                {
                    // 这个是ASCII码表里的可打印字符，占1列
                    column = 1;
                }
                else
                {
                    // 这个是多字节字符，要占两列
                    column = 2;
                }

                if (leftCols < column)
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

                document.SetCursorLogical(printRow, printColumn);
                VTCharacter character = VTCharacter.Create(ch, column);
                this.document.PrintCharacter(character);
            }
        }

        #endregion
    }
}
