using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GTerminalControl
{
    public class VT100 : VideoTerminal
    {
        #region 事件

        public event Action<object, byte, byte[]> Action;

        #endregion

        #region 实例变量

        private ParseState psrState;

        #endregion

        #region 属性

        public override VTTypeEnum Type
        {
            get
            {
                return VTTypeEnum.VT100;
            }
        }

        #endregion

        #region 构造方法

        public VT100()
        {
            this.psrState = new ParseState();
            this.psrState.StateTable = VTPrsTbl.AnsiTable;
        }

        #endregion

        #region 公开接口

        public override bool OnKeyDown(KeyEventArgs key, out byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void StartParsing(IVTStream stream)
        {
            Task.Factory.StartNew(this.Parse, stream);
        }

        #endregion

        #region 实例方法

        private void Parse(object state)
        {
            IVTStream stream = state as IVTStream;

            while (!stream.EOF)
            {
                //int lastState = this.psrState.State;
                //int nextState = this.psrState.State;

                //switch (nextState)
                //{

                //}
            }
        }

        #endregion
    }
}