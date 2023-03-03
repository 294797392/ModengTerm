using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Drawing
{
    /// <summary>
    /// 终端设备控制器
    /// </summary>
    public interface IVTController
    {
        /// <summary>
        /// 获取输入设备
        /// </summary>
        /// <returns></returns>
        IInputDevice GetInputDevice();

        /// <summary>
        /// 创建一个新的显示设备
        /// </summary>
        /// <returns></returns>
        IDrawingCanvas CreatePresentationDevice();

        /// <summary>
        /// 释放显示设备占用的资源
        /// </summary>
        /// <param name="device"></param>
        void ReleasePresentationDevice(IDrawingCanvas device);

        /// <summary>
        /// 切换显示设备
        /// </summary>
        /// <param name="toRemove">要移除的显示设备</param>
        /// <param name="toAdd">要增加的显示设备</param>
        void SwitchPresentaionDevice(IDrawingCanvas toRemove, IDrawingCanvas toAdd);
    }
}
