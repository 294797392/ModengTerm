using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Parser;

namespace XTerminal.Document
{
    public static class XDocumentUtils
    {
        public static VTextDecorations VTAction2TextDecoration(VTActions actions, out bool unset)
        {
            unset = false;

            switch (actions)
            {
                case VTActions.Bold: return VTextDecorations.Bold;
                case VTActions.BoldUnset: unset = true; return VTextDecorations.Bold;

                case VTActions.Underline: return VTextDecorations.Underline;
                case VTActions.UnderlineUnset: unset = true; return VTextDecorations.Underline;

                case VTActions.Italics: return VTextDecorations.Italics;
                case VTActions.ItalicsUnset: unset = true; return VTextDecorations.Italics;

                case VTActions.DoublyUnderlined: return VTextDecorations.DoublyUnderlined;
                case VTActions.DoublyUnderlinedUnset: unset = true; return VTextDecorations.DoublyUnderlined;

                case VTActions.Background: return VTextDecorations.Background;
                case VTActions.DefaultBackground: unset = true; return VTextDecorations.Background;

                case VTActions.Foreground: return VTextDecorations.Foreground;
                case VTActions.DefaultForeground: unset = true; return VTextDecorations.Foreground;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
