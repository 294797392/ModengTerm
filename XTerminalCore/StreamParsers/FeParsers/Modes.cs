using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore
{
    /// <summary>
    /// SM控制命令里的参数
    /// NOTE 
    /// Private modes may be implemented using private parameters, see 5.4.1 and 7.4. 
    /// </summary>
    public class Modes
    {
        /// <summary>
        /// GUARDED AREA TRANSFER MODE
        /// </summary>
        public static readonly byte GATM = 1;

        /// <summary>
        /// KEYBOARD ACTION MODE
        /// </summary>
        public static readonly byte KAM = 2;

        /// <summary>
        /// CONTROL REPRESENTATION MODE
        /// </summary>
        public static readonly byte CRM = 3;

        /// <summary>
        /// INSERTION REPLACEMENT MODE
        /// </summary>
        public static readonly byte IRM = 4;

        /// <summary>
        /// STATUS REPORT TRANSFER MODE
        /// </summary>
        public static readonly byte SRTM = 5;

        /// <summary>
        /// ERASURE MODE
        /// </summary>
        public static readonly byte ERM = 6;

        /// <summary>
        /// LINE EDITING MODE
        /// </summary>
        public static readonly byte VEM = 7;

        /// <summary>
        /// BI-DIRECTIONAL SUPPORT MODE
        /// </summary>
        public static readonly byte BDSM = 8;

        /// <summary>
        /// DEVICE COMPONENT SELECT MODE
        /// </summary>
        public static readonly byte DCSM = 9;

        /// <summary>
        /// CHARACTER EDITING MODE
        /// </summary>
        public static readonly byte HEM = 10;

        /// <summary>
        /// POSITIONING UNIT MODE 
        /// </summary>
        public static readonly byte PUM = 11;

        /// <summary>
        /// SEND/RECEIVE MODE
        /// </summary>
        public static readonly byte SRM = 12;

        /// <summary>
        /// FORMAT EFFECTOR ACTION MODE
        /// </summary>
        public static readonly byte FEAM = 13;

        /// <summary>
        /// FORMAT EFFECTOR TRANSFER MODE 
        /// </summary>
        public static readonly byte FETM = 14;

        /// <summary>
        /// MULTIPLE AREA TRANSFER MODE
        /// </summary>
        public static readonly byte MATM = 15;

        /// <summary>
        /// TRANSFER TERMINATION MODE
        /// </summary>
        public static readonly byte TTM = 16;

        /// <summary>
        /// SELECTED AREA TRANSFER MODE
        /// </summary>
        public static readonly byte SATM = 17;

        /// <summary>
        /// TABULATION STOP MODE
        /// </summary>
        public static readonly byte TSM = 18;

        /// <summary>
        /// GRAPHIC RENDITION COMBINATION
        /// </summary>
        public static readonly byte GRCM = 21;

        /// <summary>
        /// ZERO DEFAULT MODE
        /// </summary>
        public static readonly byte ZDM = 22;
    }
}