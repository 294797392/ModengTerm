using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal;
using XTerminal.Document;

namespace ModengTerm.Rendering
{
    public class DrawingMatchesLine : DrawingObject
    {
        private VTMatchesLine matchesText;

        protected override void OnInitialize(VTDocumentElement documentElement)
        {
            this.matchesText = documentElement as VTMatchesLine;
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            foreach (VTFormattedText textBlock in this.matchesText.MatchedBlocks)
            {
                FormattedText formattedText = this.CreateFormattedText(textBlock, dc);

                dc.DrawText(formattedText, new Point(textBlock.OffsetX, textBlock.OffsetY));
            }
        }

        private FormattedText CreateFormattedText(VTFormattedText textData, DrawingContext dc)
        {
            DrawingLine drawingLine = this.matchesText.TextLine.DrawingObject as DrawingLine;
            VTextLine textLine = this.matchesText.TextLine;

            FormattedText formattedText = new FormattedText(textData.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, drawingLine.typeface,
                textLine.Style.FontSize, drawingLine.foreground, null, TextFormattingMode.Display, App.PixelsPerDip);

            #region 画文本装饰

            foreach (VTextAttribute textAttribute in textData.Attributes)
            {
                switch (textAttribute.Attribute)
                {
                    case VTextAttributes.Foreground:
                        {
                            VTColor color = textAttribute.Parameter as VTColor;
                            Brush brush = DrawingUtils.GetBrush(color, textLine.Style.ColorTable);
                            formattedText.SetForegroundBrush(brush, textAttribute.StartIndex, textAttribute.Count);
                            break;
                        }

                    case VTextAttributes.Background:
                        {
                            // 背景颜色最后画, 因为文本的粗细会影响到背景颜色的大小
                            break;
                        }

                    case VTextAttributes.Bold:
                        {
                            //formattedText.SetFontWeight(FontWeights.Bold, startIndex, count);
                            break;
                        }

                    case VTextAttributes.Italics:
                        {
                            formattedText.SetFontStyle(FontStyles.Italic, textAttribute.StartIndex, textAttribute.Count);
                            break;
                        }

                    case VTextAttributes.Underline:
                        {
                            formattedText.SetTextDecorations(TextDecorations.Underline, textAttribute.StartIndex, textAttribute.Count);
                            break;
                        }

                    default:
                        break;
                }
            }

            #endregion

            // 画背景颜色
            // 背景颜色要在最后画，因为文本的粗细会影响到背景颜色的大小
            foreach (VTextAttribute textAttribute in textData.Attributes)
            {
                if (textAttribute.Attribute != VTextAttributes.Background)
                {
                    continue;
                }

                VTColor color = textAttribute.Parameter as VTColor;
                Brush brush = DrawingUtils.GetBrush(color, textLine.Style.ColorTable);
                Geometry geometry = formattedText.BuildHighlightGeometry(new Point(textData.OffsetX, textData.OffsetY), textAttribute.StartIndex, textAttribute.Count);
                dc.DrawRectangle(brush, DrawingUtils.TransparentPen, geometry.Bounds);
            }

            return formattedText;
        }
    }
}
