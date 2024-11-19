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

        /// <summary>
        /// 是否是第一次接收数据
        /// </summary>
        private bool firstReceive;
        private bool lineFeed;

        private bool renderAtNewLine;

        /// <summary>
        /// 是否渲染了一次
        /// </summary>
        private bool renderOnce;

        public TextRenderer(VideoTerminal vt) : 
            base(vt)
        {
            this.document = vt.MainDocument;
        }

        public override void Initialize()
        {
            this.renderAtNewLine = this.session.GetOption<bool>(OptionKeyEnum.TERM_ADVANCE_RENDER_AT_NEWLINE);
        }

        public override void OnInteractionStateChanged(InteractionStateEnum istate)
        {
            switch (istate)
            {
                case InteractionStateEnum.UserInput:
                    {
                        // 用户输入的之后，下一次收到的数据就是第一次接收的数据
                        this.firstReceive = true;
                        break;
                    }

                case InteractionStateEnum.Receive:
                    {
                        // 如果是第一次接收数据，那么就换行
                        if (this.firstReceive)
                        {
                            this.firstReceive = false;
                            this.lineFeed = true;
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public override void Release()
        {
        }

        public override void Render(byte[] bytes, int length)
        {
            VTDocument document = this.document;
            int viewportColumn = document.ViewportColumn;
            int viewportRow = document.ViewportRow;

            // 收到新的数据之后在新行显示
            if (this.renderAtNewLine)
            {
                if (!this.renderOnce)
                {
                    // 渲染第一次的时候不换行
                    this.renderOnce = true;
                    this.lineFeed = false;
                }
                else
                {
                    if (this.lineFeed)
                    {
                        this.videoTerminal.CarriageReturn();
                        this.videoTerminal.LineFeed();
                        this.lineFeed = false;
                    }
                }
            }

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
