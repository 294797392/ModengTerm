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
    public class VTMatchesLine : VTextElement
    {
        #region 实例变量

        private VTextLine textLine;
        private List<int> matchedIndexs;
        private List<VTCharacter> characters;

        private bool dirty;

        #endregion

        #region 属性

        public override VTDocumentElements Type => VTDocumentElements.MatchesText;

        /// <summary>
        /// 该文本块关联的文本行
        /// </summary>
        public VTextLine TextLine
        {
            get { return this.textLine; }
            set
            {
                this.textLine = value;
                this.SetDirty(true);
            }
        }

        /// <summary>
        /// 起始字符索引
        /// </summary>
        public List<int> MatchedIndexs
        {
            get { return this.matchedIndexs; }
            set
            {
                if (this.matchedIndexs != value)
                {
                    this.matchedIndexs = value;

                    this.SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 每个匹配项的长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 当前终端的背景颜色
        /// </summary>
        public VTColor Background { get; set; }

        /// <summary>
        /// 当前终端的前景色
        /// </summary>
        public VTColor Foreground { get; set; }

        /// <summary>
        /// 匹配的文本块
        /// </summary>
        public List<VTFormattedText> MatchedBlocks { get; private set; }

        #endregion

        #region 构造方法

        public VTMatchesLine(VTDocument owner) :
            base(owner)
        {
            this.characters = new List<VTCharacter>();
            this.MatchedBlocks = new List<VTFormattedText>();
        }

        #endregion

        #region 实例方法

        private void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;

                base.SetRenderDirty(true);
            }
        }

        #endregion

        #region VTextElement

        public override void RequestInvalidate()
        {
            if (!this.dirty)
            {
                return;
            }

            this.MatchedBlocks.Clear();

            // 先拷贝要显示的字符
            VTUtils.CopyCharacter(this.TextLine.Characters, this.characters);

            foreach (int startIndex in this.MatchedIndexs)
            {
                #region 更新前景色和背景色

                // 设置字符的高亮颜色，这里直接把前景色和背景色做反色
                // TODO：有可能背景不是纯色，而是图片或者视频
                for (int i = 0; i < this.Length; i++)
                {
                    VTCharacter character = this.characters[startIndex + i];

                    VTextAttributeState foregroundAttribute = character.AttributeList[(int)VTextAttributes.Foreground];
                    foregroundAttribute.Enabled = true;
                    foregroundAttribute.Parameter = VTColor.DarkGreen;// this.Background;

                    VTextAttributeState backgroundAttribute = character.AttributeList[(int)VTextAttributes.Background];
                    backgroundAttribute.Enabled = true;
                    backgroundAttribute.Parameter = VTColor.DarkGreen;// this.Foreground;
                }

                #endregion

                VTRect rect = this.textLine.MeasureTextBlock(startIndex, this.Length);

                VTFormattedText formattedText = VTUtils.CreateFormattedText(this.characters, startIndex, this.Length);
                formattedText.OffsetX = rect.Left;
                formattedText.OffsetY = this.textLine.OffsetY;

                this.MatchedBlocks.Add(formattedText);
            }

            base.RequestInvalidate();

            this.SetDirty(false);
        }

        #endregion
    }
}
