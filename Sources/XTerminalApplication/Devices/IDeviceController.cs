using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalDevice.Interface
{
    /// <summary>
    /// 终端设备控制器
    /// </summary>
    public interface IDeviceController
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
        IPresentationDevice CreatePresentationDevice();

        /// <summary>
        /// 释放显示设备占用的资源
        /// </summary>
        /// <param name="device"></param>
        void ReleasePresentationDevice(IPresentationDevice device);

        /// <summary>
        /// 切换显示设备
        /// </summary>
        /// <param name="toRemove">要移除的显示设备</param>
        /// <param name="toAdd">要增加的显示设备</param>
        void SwitchPresentaionDevice(IPresentationDevice toRemove, IPresentationDevice toAdd);
    }
}
