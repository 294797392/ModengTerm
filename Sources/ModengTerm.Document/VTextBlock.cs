using ModengTerm.Document.Drawing;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 用来渲染一个VTextLine里的文本块
    /// </summary>
    public class VTextBlock : VTElement
    {
        #region 实例变量

        private VTextLine textLine;

        private VTMatches matches;

        #endregion

        #region 属性

        public override DrawingObjectTypes Type => DrawingObjectTypes.TextBlock;

        /// <summary>
        /// 该文本块关联的文本行
        /// </summary>
        public VTextLine TextLine
        {
            get { return textLine; }
            set
            {
                if (textLine != value)
                {
                    textLine = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        /// <summary>
        /// 存储匹配的元素列表
        /// </summary>
        public VTMatches Matches
        {
            get { return this.matches; }
            set
            {
                if (this.matches != value)
                {
                    this.matches = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        #endregion

        #region 构造方法

        public VTextBlock(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region VTElement

        protected override void OnInitialize(IDrawingObject drawingObject)
        {
        }

        protected override void OnRelease()
        {
        }

        protected override void OnRender()
        {
            IDrawingTextBlock drawingTextBlock = this.DrawingObject as IDrawingTextBlock;

            // 先拷贝要显示的字符
            List<VTCharacter> characters = new List<VTCharacter>();
            VTUtils.CopyCharacter(textLine.Characters, characters);

            #region 更新前景色和背景色

            // 设置字符的高亮颜色，这里直接把前景色和背景色做反色
            for (int i = 0; i < matches.Length; i++)
            {
                VTCharacter character = characters[matches.Index + i];

                VTUtils.SetTextAttribute(VTextAttributes.Foreground, true, ref character.Attribute);
                VTUtils.SetTextAttribute(VTextAttributes.Background, true, ref character.Attribute);
                character.Foreground = VTColor.CreateFromRgbKey(textLine.Style.BackgroundColor);
                character.Background = VTColor.CreateFromRgbKey(textLine.Style.ForegroundColor);
            }

            #endregion

            VTRect rect = textLine.MeasureTextBlock(matches.Index, matches.Length);

            VTFormattedText formattedText = VTUtils.CreateFormattedText(characters, matches.Index, matches.Length);
            formattedText.OffsetX = rect.Left;
            formattedText.Style = textLine.Style;

            drawingTextBlock.FormattedText = formattedText;
        }

        #endregion
    }
}
