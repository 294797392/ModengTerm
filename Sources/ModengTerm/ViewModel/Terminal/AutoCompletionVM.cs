using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Document;
using ModengTerm.Document.EventData;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Parsing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xaml;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel.Terminal
{
    /// <summary>
    /// 实现自动补全功能
    /// </summary>
    public class AutoCompletionVM : ViewModelBase//, IKeyboardHook
    {
        #region 常量

        private static readonly string[] Splitters = new string[]
        {
            "(", ")", "-", "=", "+",
            "\\", "/", ",", ".", "|", "\"", ";",
            "<", ">", "[", "]", "{", "}",
            " "
        };

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("AutoCompletionVM");

        #endregion

        #region 实例变量

        private ShellSessionVM shellSession;
        private IVideoTerminal terminal;
        private int waitPrintLen;       // 用户输入完之后，是否需要等待把用户输入的内容打印出来
        private AutoCompletionSource source;
        private bool enabled;
        private bool bsEntered;
        private bool isOpen;

        /// <summary>
        /// 是否显示下拉列表
        /// </summary>
        private bool showAcList;

        /// <summary>
        /// 当前搜索的关键字
        /// </summary>
        private string keyword;

        #endregion

        #region 属性

        /// <summary>
        /// 是否启用自动完成功能
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;

                    throw new RefactorImplementedException();

                    //if (value)
                    //{
                    //    terminal.OnPrint += Terminal_OnPrint;
                    //    terminal.OnLineFeed += Terminal_OnLineFeed;
                    //    //terminal.OnKeyboardInput += Terminal_OnKeyboardInput;
                    //    //terminal.OnC0ActionExecuted += Terminal_OnC0ActionExecuted;
                    //    terminal.MainDocument.Rendering += VTDocument_Rendering;
                    //    terminal.AlternateDocument.Rendering += VTDocument_Rendering;
                    //}
                    //else
                    //{
                    //    terminal.OnPrint -= Terminal_OnPrint;
                    //    terminal.OnLineFeed -= Terminal_OnLineFeed;
                    //    //terminal.OnKeyboardInput -= Terminal_OnKeyboardInput;
                    //    //terminal.OnC0ActionExecuted -= Terminal_OnC0ActionExecuted;
                    //    terminal.MainDocument.Rendering -= VTDocument_Rendering;
                    //    terminal.AlternateDocument.Rendering -= VTDocument_Rendering;
                    //}
                }
            }
        }

        /// <summary>
        /// 自动完成列表里的数据
        /// </summary>
        public BindableCollection<string> Items { get; private set; }

        /// <summary>
        /// 获取自动补全功能所对应的终端
        /// </summary>
        public IVideoTerminal VideoTerminal { get { return terminal; } }

        /// <summary>
        /// 控制是否打开自动完成列表
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                if (isOpen != value)
                {
                    isOpen = value;
                    NotifyPropertyChanged("IsOpen");
                }
            }
        }

        #endregion

        #region 公开接口

        public void Initialize(ShellSessionVM shellSession)
        {
            this.shellSession = shellSession;
            terminal = shellSession.VideoTerminal;
            source = new AutoCompletionSource();
            Items = new BindableCollection<string>();
        }

        public void Release()
        {
            Enabled = false;
            shellSession = null;
            terminal = null;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 打开自动完成列表
        /// </summary>
        private void Open()
        {
            keyword = string.Empty;

            VTDocument document = terminal.ActiveDocument;
            VTextLine textLine = document.ActiveLine;
            int row = document.Cursor.Row;
            int col = document.Cursor.Column;

            List<VTCharacter> characters = textLine.Characters;

            int startIndex, count;
            VTDocUtils.GetSegement(characters, col - 1, out startIndex, out count);
            if (count == 0)
            {
                Close();
                return;
            }

            string text = VTDocUtils.CreatePlainText(characters, startIndex, count);
            if (string.IsNullOrEmpty(text))
            {
                Close();
                return;
            }

            keyword = text;

            // 根据关键字搜索自动完成列表项
            List<string> autoCompleteItems = source.SearchItems(text);
            if (autoCompleteItems.Count() == 0)
            {
                // 没有搜索到自动完成列表
                Close();
            }
            else
            {
                //logger.FatalFormat("打开自动完成列表, {0}", autoCompleteItems.Count);
                Items.Clear();
                Items.AddRange(autoCompleteItems);
                IsOpen = true;
            }
        }

        /// <summary>
        /// 关闭自动完成列表
        /// </summary>
        private void Close()
        {
            logger.DebugFormat("关闭自动完成列表");

            keyword = string.Empty;
            Items.Clear();
            IsOpen = false;
        }

        #endregion

        #region 事件处理器

        private void Terminal_OnKeyboardInput(IVideoTerminal arg1, VTKeyboardInput kbdInput)
        {
            // 先判断用户输入的数据是否可以触发显示自动完成列表

            if (kbdInput.FromIMEInput)
            {
                // 用户通过输入法输入数据
                waitPrintLen = kbdInput.Text.Length;
            }
            else
            {
                // 用户输入的是按键，判断按键是否是可打印字符

                byte[] send = kbdInput.SendBytes;

                VTKeys key = kbdInput.Key;
                switch (key)
                {
                    case VTKeys.Back:
                        {
                            bsEntered = true;
                            break;
                        }

                    default:
                        {
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

                            waitPrintLen = 1;

                            break;
                        }
                }
            }

            logger.DebugFormat("OnKyeboardInput, {0}", waitPrintLen);

            // 这里说明可以显示自动完成列表了
        }

        private void Terminal_OnLineFeed(IVideoTerminal arg1, bool isAlternate, int oldPhysicsRow, VTHistoryLine historyLine)
        {
            string[] strings;
            VTDocUtils.Split(historyLine.Characters, Splitters, out strings);

            if (strings == null || strings.Length == 0)
            {
                return;
            }

            foreach (string str in strings)
            {
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }

                //logger.InfoFormat("新建项, {0}", str);

                source.AddItem(str);
            }
        }

        private void Terminal_OnPrint(IVideoTerminal arg1)
        {
            if (waitPrintLen <= 0)
            {
                return;
            }

            waitPrintLen--;

            logger.DebugFormat("OnPrint, {0}", waitPrintLen);

            if (waitPrintLen == 0)
            {
                logger.DebugFormat("showAcList");

                // 用户输入的字符已经接收完毕了，可以执行自动完成列表的显示逻辑了
                // 在下次渲染结束的时候显示自动完成列表
                showAcList = true;
            }
        }

        private void Terminal_OnC0ActionExecuted(IVideoTerminal arg1, ASCIITable ascii)
        {
            switch (ascii)
            {
                case ASCIITable.BS:
                    {
                        // 确保是通过键盘输入触发的Backspace
                        if (!bsEntered)
                        {
                            return;
                        }

                        bsEntered = false;

                        // 使用退格键移动光标之后，等渲染完之后显示自动完成列表
                        showAcList = true;

                        // 在下次渲染结束的时候显示自动完成列表
                        break;
                    }

                case ASCIITable.LF:
                case ASCIITable.FF:
                case ASCIITable.VT:
                    {
                        // 换行就直接关闭自动完成列表
                        Close();
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private void VTDocument_Rendering(VTDocument document, VTRenderData renderData)
        {
            if (showAcList)
            {
                Open();

                showAcList = false;
            }
        }

        public bool OnKeyDown(VTKeys vtKey)
        {
            // 如果没显示自动完成列表，那么不需要处理，直接返回
            if (!IsOpen)
            {
                return true;
            }

            switch (vtKey)
            {
                case VTKeys.Up:
                    {
                        Items.SelectPrevious();
                        return false;
                    }

                case VTKeys.Down:
                    {
                        Items.SelectNext();
                        return false;
                    }

                case VTKeys.Escape:
                    {
                        Close();
                        return false;
                    }

                case VTKeys.Tab:
                case VTKeys.Enter:
                    {
                        string text = Items.SelectedItem;
                        if (string.IsNullOrEmpty(text))
                        {
                            Close();
                            return true;
                        }

                        // 把当前搜索的关键字替换成用户在自动完成列表里选中的项

                        string textToSend = text.Substring(keyword.Length);

                        throw new RefactorImplementedException();
                        //shellSession.SendText(textToSend);

                        Close();

                        return false;
                    }

                default:
                    {
                        return true;
                    }
            }
        }

        #endregion
    }
}
