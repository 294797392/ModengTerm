using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Renderer
{
    /// <summary>
    /// 将收到的数据按照可打印字符渲染（包含中文）
    /// 会处理换行逻辑
    /// </summary>
    public class TextRenderer : VTermRenderer
    {
        private VTDocument document;

        public TextRenderer(VideoTerminal vt) : 
            base(vt)
        {
            this.document = vt.MainDocument;
        }

        public override void Initialize()
        {
        }

        public override void Release()
        {
        }

        public override void Render(byte[] bytes, int length)
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
    }
}
