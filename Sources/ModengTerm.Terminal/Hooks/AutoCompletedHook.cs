using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.Hooks.AutoCompleted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Hooks
{
    public class AutoCompletedHook : VTHook
    {
        private static string[] Splitters = new string[]
        {
            "(", ")", "-", "=", "+",
            "\\", "/", ",", ".", "|", "\"", ";", 
            "<", ">", "[", "]", "{", "}",
        };

        private AutoCompletedSource source;

        protected override void OnInstall()
        {
            this.source = new AutoCompletedSource();
        }

        protected override void OnUnInstall()
        {
        }

        public override void OnCharacterPrint(int row, int col, VTCharacter character, List<VTCharacter> characters)
        {

        }

        public override void OnLineFeed(int number, VTHistoryLine historyLine)
        {
            string[] strings;
            VTUtils.Split(historyLine.Characters, Splitters, out strings);

            if (strings == null || strings.Length == 0) 
            {
                return;
            }

            foreach (string str in strings)
            {
                this.source.AddItem(str);
            }
        }

        public override void OnDocumentChange(VTDocument oldDocument, VTDocument newDocument)
        {
        }

        public override void OnKeyboardInput(VTKeyboardInput keyInput)
        {
        }
    }
}
