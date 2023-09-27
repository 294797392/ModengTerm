using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document.Graphics
{
    /// <summary>
    /// 用来渲染一个VTextLine里的文本块
    /// </summary>
    public class VTMatchesLine : VTDocumentElement
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
            get { return this.textLine; }
            set
            {
                if (this.textLine != value)
                {
                    this.textLine = value;
                    this.SetDirty(true);
                }
            }
        }

        public List<VTMatches> MatchesList
        {
            get { return this.matchesList; }
            set
            {
                if (this.matchesList != value)
                {
                    this.matchesList = value;
                    this.SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 匹配的文本块
        /// 用来渲染图形
        /// </summary>
        public List<VTFormattedText> TextBlocks { get; private set; }

        #endregion

        #region 构造方法

        public VTMatchesLine()
        {
            this.TextBlocks = new List<VTFormattedText>();
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

        public override void RequestInvalidate()
        {
            if (this.arrangeDirty)
            {
                this.DrawingObject.Arrange(this.OffsetX, this.OffsetY);

                this.arrangeDirty = false;
            }

            if (this.dirty)
            {
                this.TextBlocks.Clear();

                if (this.matchesList != null && this.matchesList.Count > 0)
                {
                    // 先拷贝要显示的字符
                    List<VTCharacter> characters = new List<VTCharacter>();
                    VTUtils.CopyCharacter(this.textLine.Characters, characters);

                    foreach (VTMatches matches in this.MatchesList)
                    {
                        #region 更新前景色和背景色

                        // 设置字符的高亮颜色，这里直接把前景色和背景色做反色
                        for (int i = 0; i < matches.Length; i++)
                        {
                            VTCharacter character = characters[matches.Index + i];

                            VTUtils.SetTextAttribute(VTextAttributes.Foreground, true, ref character.Attribute);
                            VTUtils.SetTextAttribute(VTextAttributes.Background, true, ref character.Attribute);
                            if (this.textLine.Style.Background.Type == (int)WallpaperTypeEnum.PureColor)
                            {
                                // 如果背景是纯色就变反色
                                character.Foreground = VTColor.CreateFromRgbKey(this.textLine.Style.Background.Uri);
                                character.Background = VTColor.CreateFromRgbKey(this.textLine.Style.Foreground);
                            }
                            else
                            {
                                // 如果背景不是纯色，那么就前景色用白色，背景色用黑色
                                character.Foreground = VTColor.CreateFromRgbKey("255,255,255");
                                character.Background = VTColor.CreateFromRgbKey("0,0,0");
                            }
                        }

                        #endregion

                        VTRect rect = this.textLine.MeasureTextBlock(matches.Index, matches.Length);

                        VTFormattedText formattedText = VTUtils.CreateFormattedText(characters, matches.Index, matches.Length);
                        formattedText.OffsetX = rect.Left;
                        formattedText.Style = this.textLine.Style;

                        this.TextBlocks.Add(formattedText);
                    }
                }

                this.DrawingObject.Draw();

                this.dirty = false;
            }
        }

        #endregion
    }
}
