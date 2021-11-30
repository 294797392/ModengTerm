using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace VideoTerminal.Controls
{
    public class TerminalTextParagraphProperties : TextParagraphProperties
    {
        #region 实例变量

        private FlowDirection flowDirection;
        private TextAlignment textAlignment;
        private double lineHeight;
        private bool firstLineInParagraph;
        private TextRunProperties defaultTextRunProperties;
        private TextWrapping textWrapping;
        private TextMarkerProperties textMarkerProperties;
        private double indent;

        #endregion

        #region 属性

        public override FlowDirection FlowDirection => this.flowDirection;

        public override TextAlignment TextAlignment => this.textAlignment;

        public override double LineHeight => this.lineHeight;

        public override bool FirstLineInParagraph => this.firstLineInParagraph;

        public override TextRunProperties DefaultTextRunProperties => this.defaultTextRunProperties;

        public override TextWrapping TextWrapping => this.textWrapping;

        public override TextMarkerProperties TextMarkerProperties => this.textMarkerProperties;

        public override double Indent => this.indent;

        #endregion

        #region 构造方法

        public TerminalTextParagraphProperties()
        {
            this.flowDirection = FlowDirection.LeftToRight;
            this.textAlignment = TextAlignment.Left;
            this.lineHeight = 0;
            this.firstLineInParagraph = false;
            this.defaultTextRunProperties = new TerminalTextRunProperties();
            this.textWrapping = TextWrapping.Wrap;
            this.textMarkerProperties = null;
            this.indent = 0;
        }

        #endregion
    }
}


