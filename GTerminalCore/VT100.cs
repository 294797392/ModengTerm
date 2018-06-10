using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GTerminalCore
{
    public class VT100 : VideoTerminal
    {
        #region 事件

        public event Action<object, byte, byte[]> Action;

        #endregion

        #region 实例变量


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
        }

        #endregion

        #region 公开接口

        public override bool OnKeyDown(KeyEventArgs key, out byte[] data)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 实例方法

        #endregion
    }
}