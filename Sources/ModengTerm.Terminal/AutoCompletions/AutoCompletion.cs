using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.AutoCompletions
{
    public class AutoCompletion
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("AutoCompletion");

        public event Action<AutoCompletion, IEnumerable<string>> AutoCompletedItemsChanged;

        private static string[] Splitters = new string[]
        {
            "(", ")", "-", "=", "+",
            "\\", "/", ",", ".", "|", "\"", ";",
            "<", ">", "[", "]", "{", "}",
        };

        private IVideoTerminal videoTerminal;
        private int waitPrintLen;       // 用户输入完之后，是否需要等待把用户输入的内容打印出来
        private AutoCompletionSource source;

        public AutoCompletion(IVideoTerminal vt)
        {
            this.videoTerminal = vt;
            this.videoTerminal.OnLineFeed += VideoTerminal_OnLineFeed;
            this.videoTerminal.OnDocumentChanged += VideoTerminal_OnDocumentChanged;
            this.videoTerminal.OnCharacterPrint += VideoTerminal_OnCharacterPrint;
            this.videoTerminal.OnKeyboardInput += VideoTerminal_OnKeyboardInput;
            this.source = new AutoCompletionSource();
        }

        public void Initialize()
        {

        }

        public void Release()
        {

        }

        #region 事件处理器

        private void VideoTerminal_OnKeyboardInput(IVideoTerminal arg1, VTKeyboardInput kbdInput)
        {
            // 先判断用户输入的数据是否可以触发显示自动完成列表

            if (kbdInput.FromIMEInput)
            {
                // 用户通过输入法输入数据
                this.waitPrintLen = kbdInput.Text.Length;
            }
            else
            {
                // 用户输入的是按键，判断按键是否是可打印字符

                byte[] send = kbdInput.SendBytes;

                VTKeys key = kbdInput.Key;

                if (send.Length == 0)
                {
                    return;
                }

                byte c = send[0];

                if (!(c >= '0' && c <= '9') &&
                    !(c >= 'a' && c <= 'z') &&
                    !(c >= 'A' && c <= 'Z'))
                {
                    return;
                }

                this.waitPrintLen = 1;
            }

            // 这里说明可以显示自动完成列表了
        }

        private void VideoTerminal_OnCharacterPrint(IVideoTerminal arg1, int row, int col, VTCharacter character, List<VTCharacter> characters)
        {

            if (this.waitPrintLen == 0)
            {
                return;
            }

            this.waitPrintLen--;

            if (this.waitPrintLen != 0)
            {
                return;
            }

            // 用户输入的字符已经接收完毕了，可以执行自动完成列表的显示逻辑了

            int startIndex, count;
            VTUtils.GetSegement(characters, col, out startIndex, out count);
            string text = VTUtils.CreatePlainText(characters, startIndex, count);
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            IEnumerable<string> autoCompleteItems = this.source.SearchItems(text);
            if (autoCompleteItems.Count() == 0)
            {
                return;
            }

            this.AutoCompletedItemsChanged?.Invoke(this, autoCompleteItems);
        }

        private void VideoTerminal_OnLineFeed(IVideoTerminal arg1, VTHistoryLine historyLine)
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

        private void VideoTerminal_OnDocumentChanged(IVideoTerminal arg1, VTDocument oldDocument, VTDocument newDocument)
        {
        }

        #endregion
    }
}
