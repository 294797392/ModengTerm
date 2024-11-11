using ModengTerm.Terminal.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Triggers
{
    /// <summary>
    /// 执行触发器的容器
    /// </summary>
    public class TriggerExecutor
    {
        private IVideoTerminal terminal;
        private bool enabled;

        /// <summary>
        /// 要执行的触发器
        /// </summary>
        public Trigger Trigger { get; private set; }

        /// <summary>
        /// 启用或者禁用触发器
        /// </summary>
        /// <param name="isEnable">是否启用触发器</param>
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
                    }
                    else 
                    {
                    }
                }
            }
        }


        public TriggerExecutor(Trigger trigger, IVideoTerminal terminal)
        {
            this.terminal = terminal;
        }
    }
}




