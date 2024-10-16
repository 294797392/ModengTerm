using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Parsing;
using ModengTerm.Terminal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xaml;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals
{
    /// <summary>
    /// 实现自动补全功能
    /// </summary>
    public class AutoCompletionVM : ViewModelBase, IKeyboardHook
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
        private double offsetX;
        private double offsetY;
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
            get { return this.enabled; }
            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;

                    if (value)
                    {
                        this.terminal.OnPrint += Terminal_OnPrint;
                        this.terminal.OnLineFeed += Terminal_OnLineFeed;
                        this.terminal.OnKeyboardInput += Terminal_OnKeyboardInput;
                        this.terminal.OnC0ActionExecuted += Terminal_OnC0ActionExecuted;
                        this.terminal.OnRendered += Terminal_OnRendered;
                    }
                    else
                    {
                        this.terminal.OnPrint -= Terminal_OnPrint;
                        this.terminal.OnLineFeed -= Terminal_OnLineFeed;
                        this.terminal.OnKeyboardInput -= Terminal_OnKeyboardInput;
                        this.terminal.OnC0ActionExecuted -= Terminal_OnC0ActionExecuted;
                        this.terminal.OnRendered -= Terminal_OnRendered;
                    }
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
        public IVideoTerminal VideoTerminal { get { return this.terminal; } }

        /// <summary>
        /// 控制是否打开自动完成列表
        /// </summary>
        public bool IsOpen
        {
            get { return this.isOpen;}
            set 
            {
                if (this.isOpen != value) 
                {
                    this.isOpen = value;
                    this.NotifyPropertyChanged("IsOpen");
                }
            }
        }

        public double OffsetX
        {
            get { return this.offsetX; }
            private set
            {
                if (this.offsetX != value)
                {
                    this.offsetX = value;
                    this.NotifyPropertyChanged("OffsetX");
                }
            }
        }

        public double OffsetY
        {
            get { return this.offsetY; }
            private set
            {
                if (this.offsetY != value)
                {
                    this.offsetY = value;
                    this.NotifyPropertyChanged("OffsetY");
                }
            }
        }

        #endregion

        #region 公开接口

        public void Initialize(ShellSessionVM shellSession)
        {
            this.shellSession = shellSession;
            this.terminal = shellSession.VideoTerminal;
            this.source = new AutoCompletionSource();
            this.Items = new BindableCollection<string>();
        }

        public void Release()
        {
            this.Enabled = false;
            this.shellSession = null;
            this.terminal = null;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 打开自动完成列表
        /// </summary>
        private void Open()
        {
            this.keyword = string.Empty;

            VTDocument document = this.terminal.ActiveDocument;
            VTextLine textLine = document.ActiveLine;
            int row = document.Cursor.Row;
            int col = document.Cursor.Column;

            List<VTCharacter> characters = textLine.Characters;

            int startIndex, count;
            VTUtils.GetSegement(characters, col - 1, out startIndex, out count);
            if (count == 0)
            {
                return;
            }

            string text = VTUtils.CreatePlainText(characters, startIndex, count);
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            this.keyword = text;

            // 根据关键字搜索自动完成列表项
            List<string> autoCompleteItems = this.source.SearchItems(text);
            if (autoCompleteItems.Count() == 0)
            {
                // 没有搜索到自动完成列表
                this.Close();
            }
            else
            {
                logger.DebugFormat("打开自动完成列表, {0}", autoCompleteItems.Count);
                this.Items.Clear();
                this.Items.AddRange(autoCompleteItems);
                this.IsOpen = true;
            }
        }

        /// <summary>
        /// 关闭自动完成列表
        /// </summary>
        private void Close()
        {
            logger.DebugFormat("关闭自动完成列表");

            this.keyword = string.Empty;
            this.Items.Clear();
            this.IsOpen = false;
        }

        #endregion

        #region 事件处理器

        private void Terminal_OnKeyboardInput(IVideoTerminal arg1, VTKeyboardInput kbdInput)
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
                switch (key)
                {
                    case VTKeys.Back:
                        {
                            this.bsEntered = true;
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

                            this.waitPrintLen = 1;

                            break;
                        }
                }
            }

            // 这里说明可以显示自动完成列表了
        }

        private void Terminal_OnLineFeed(IVideoTerminal arg1, VTHistoryLine historyLine)
        {
            string[] strings;
            VTUtils.Split(historyLine.Characters, Splitters, out strings);

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

                this.source.AddItem(str);
            }
        }

        private void Terminal_OnPrint(IVideoTerminal arg1)
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
            // 在下次渲染结束的时候显示自动完成列表
            this.showAcList = true;
        }

        private void Terminal_OnC0ActionExecuted(IVideoTerminal arg1, ASCIITable ascii)
        {
            switch (ascii)
            {
                case ASCIITable.BS:
                    {
                        // 确保是通过键盘输入触发的Backspace
                        if (!this.bsEntered)
                        {
                            return;
                        }

                        this.bsEntered = false;

                        this.showAcList = true;

                        // 在下次渲染结束的时候显示自动完成列表
                        break;
                    }

                case ASCIITable.LF:
                case ASCIITable.FF:
                case ASCIITable.VT:
                    {
                        // 换行就直接关闭自动完成列表
                        this.Close();
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        private void Terminal_OnRendered(IVideoTerminal vt)
        {
            if (this.showAcList)
            {
                this.Open();

                this.showAcList = false;
            }
        }

        public bool OnKeyDown(VTKeys vtKey)
        {
            // 如果没显示自动完成列表，那么不需要处理，直接返回
            if (!this.IsOpen)
            {
                return true;
            }

            switch (vtKey)
            {
                case VTKeys.Up:
                    {
                        this.Items.SelectPrevious();
                        return false;
                    }

                case VTKeys.Down:
                    {
                        this.Items.SelectNext();
                        return false;
                    }

                case VTKeys.Escape:
                    {
                        this.Close();
                        return false;
                    }

                case VTKeys.Tab:
                case VTKeys.Enter:
                    {
                        string text = this.Items.SelectedItem;
                        if (string.IsNullOrEmpty(text))
                        {
                            this.Close();
                            return true;
                        }

                        // 把当前搜索的关键字替换成用户在自动完成列表里选中的项

                        string textToSend = text.Substring(this.keyword.Length);

                        this.shellSession.SendText(textToSend);

                        this.Close();

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
