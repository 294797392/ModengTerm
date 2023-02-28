/*----------------------------------------------------------------
// Copyright (C) Suzhou HYC Technology Co.,LTD
// 版权所有。
//
// =================================
// CLR版本 ：4.0.30319.42000
// 命名空间 ：XTerminalParser
// 文件名称 ：VTKeypadMode.cs
// =================================
// 创 建 者 ：hyc-zyf
// 创建日期 ：2023/2/28 10:50:18
// 功能描述 ：
// 使用说明 ：
//
//
// 创建标识：hyc-zyf-2023/2/28 10:50:18
//
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalParser
{
    public enum VTKeypadMode
    {
        ApplicationMode,

        /// <summary>
        /// DECKPNM
        /// The auxiliary keypad keys will send ASCII codes corresponding to the characters engraved on the keys
        /// </summary>
        NumericMode
    }
}
