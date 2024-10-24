﻿using ModengTerm.Base.DataModels;
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
    /// 将收到的数据渲染为hexdump形式的格式
    /// </summary>
    public class HexdumpRenderer : VTermRenderer
    {
        #region 实例变量

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

        #region ShellRenderer

        public override void Initialize()
        {
            this.renderAtNewLine = this.session.GetOption<bool>(OptionKeyEnum.TERM_ADVANCE_RENDER_AT_NEWLINE);
        }

        public override void Release()
        {
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

                document.SetCursor(printRow, printColumn);
                VTCharacter character1 = VTCharacter.CreateNull();
                document.PrintCharacter(character1);

                document.SetCursor(printRow, printColumn + 1);
                VTCharacter character2 = VTCharacter.Create(hexstr[0], 1);
                document.PrintCharacter(character2);

                document.SetCursor(printRow, printColumn + 2);
                VTCharacter character3 = VTCharacter.Create(hexstr[1], 1);
                document.PrintCharacter(character3);

                document.SetCursor(printRow, printColumn + 3);
            }
        }

        #endregion
    }
}
