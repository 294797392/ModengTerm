using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 用来渲染一个VTextLine里的文本块
    /// </summary>
    public class VTMatchesLine : VTDocumentElement<IDrawingMatchesLine>
    {
        #region 实例变量

        private VTextLine textLine;

        private List<VTMatches> matchesList;

        private bool dirty;

        #endregion

        #region 属性

        public override VTDocumentElements Type => VTDocumentElements.MatchesLine;

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
                    SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 存储匹配的元素列表
        /// </summary>
        public List<VTMatches> MatchesList
        {
            get { return matchesList; }
            set
            {
                if (matchesList != value)
                {
                    matchesList = value;
                    SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 匹配的文本块
        /// 用来渲染图形
        /// </summary>
        public List<VTFormattedText> TextBlocks
        {
            get { return this.DrawingObject.TextBlocks; }
            private set
            {
                if (this.DrawingObject.TextBlocks != value)
                {
                    this.DrawingObject.TextBlocks = value;
                }
            }
        }

        #endregion

        #region 构造方法

        public VTMatchesLine(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region 实例方法

        private void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;
            }
        }

        #endregion

        #region VTDocumentElement

        protected override void OnInitialize()
        {
            TextBlocks = new List<VTFormattedText>();
        }

        protected override void OnRelease()
        {
        }

        public override void RequestInvalidate()
        {
            if (this.GetDirtyFlags(VTDirtyFlags.PositionDirty))
            {
                this.DrawingObject.Arrange(this.OffsetX, this.OffsetY);
            }

            if (dirty)
            {
                this.TextBlocks.Clear();

                if (matchesList != null && matchesList.Count > 0)
                {
                    // 先拷贝要显示的字符
                    List<VTCharacter> characters = new List<VTCharacter>();
                    VTUtils.CopyCharacter(textLine.Characters, characters);

                    foreach (VTMatches matches in MatchesList)
                    {
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

                        this.TextBlocks.Add(formattedText);
                    }
                }

                this.DrawingObject.Draw();

                dirty = false;
            }

            this.ResetDirtyFlags();
        }

        #endregion
    }
}
