using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using XTerminalCore.Invocations;

namespace XTerminal.Controls
{
    /// <summary>
    /// 终端显示器
    /// </summary>
    public class TermDisplayer : Control
    {
        private TermLineList termLines;

        public TermDisplayer()
        {
        }

        private void InitializeScreen()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.termLines = base.Template.FindName("PART_VisualLines", this) as TermLineList;

            this.InitializeScreen();
        }

        /// <summary>
        /// 执行一个ControlFunction调用
        /// </summary>
        /// <param name="invocation"></param>
        public void ExecInvocation(IInvocation invocation)
        {

        }

        /// <summary>
        /// 在显示器上显示一个字符
        /// </summary>
        /// <param name="c"></param>
        public void ProcessChar(char c)
        {
            
        }
    }
}