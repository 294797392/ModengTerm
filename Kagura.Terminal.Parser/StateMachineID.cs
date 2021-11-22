using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    public enum StateMachineID
    {
        CASE_GROUND_STATE = 0,
        CASE_IGNORE = 1,

        /// <summary>
        /// 响铃
        /// BEL is used when there is a need to call for attention, it may control alarm or attention devices. 
        /// </summary>
        CASE_BELL = 2,

        /// <summary>
        /// 退格
        /// BS causes the active data position to be moved one character position in the data component in the 
        /// direction opposite to that of the implicit movement.
        /// The direction of the implicit movement depends on the parameter value of SELECT IMPLICIT 
        /// MOVEMENT DIRECTION(SIMD). 
        /// </summary>
        CASE_BS = 3,

        /// <summary>
        /// CarriageReturn
        /// 回车，把光标移动到第一列
        /// </summary>
        CASE_CR = 4,
        CASE_ESC = 5,
        CASE_VMOT = 6,
        CASE_TAB = 7,
        CASE_SI = 8,
        CASE_SO = 9,
        CASE_SCR_STATE = 10,
        CASE_SCS0_STATE = 11,
        CASE_SCS1_STATE = 12,
        CASE_SCS2_STATE = 13,
        CASE_SCS3_STATE = 14,
        CASE_ESC_IGNORE = 15,
        CASE_ESC_DIGIT = 16,
        CASE_ESC_SEMI = 17,
        CASE_DEC_STATE = 18,
        CASE_ICH = 19,
        CASE_CUU = 20,
        CASE_CUD = 21,
        CASE_CUF = 22,
        CASE_CUB = 23,
        CASE_CUP = 24,
        CASE_ED = 25,
        CASE_EL = 26,
        CASE_IL = 27,
        CASE_DL = 28,
        CASE_DCH = 29,
        CASE_DA1 = 30,
        CASE_TRACK_MOUSE = 31,
        CASE_TBC = 32,
        CASE_SET = 33,
        CASE_RST = 34,
        CASE_SGR = 35,
        CASE_CPR = 36,
        CASE_DECSTBM = 37,
        CASE_DECREQTPARM = 38,
        CASE_DECSET = 39,
        CASE_DECRST = 40,
        CASE_DECALN = 41,
        CASE_GSETS = 42,
        CASE_DECSC = 43,
        CASE_DECRC = 44,
        CASE_DECKPAM = 45,
        CASE_DECKPNM = 46,
        CASE_IND = 47,
        CASE_NEL = 48,
        CASE_HTS = 49,
        CASE_RI = 50,
        CASE_SS2 = 51,
        CASE_SS3 = 52,
        CASE_CSI_STATE = 53,

        #region ESC子状态
        /// <summary>
        /// Operating System Commands
        /// 进入OSC模式
        /// </summary>
        SMID_OSC = 54,
        #endregion

        CASE_RIS = 55,
        CASE_LS2 = 56,
        CASE_LS3 = 57,
        CASE_LS3R = 58,
        CASE_LS2R = 59,
        CASE_LS1R = 60,
        CASE_PRINT = 61,
        CASE_XTERM_SAVE = 62,
        CASE_XTERM_RESTORE = 63,
        CASE_XTERM_TITLE = 64,
        CASE_DECID = 65,
        CASE_HP_MEM_LOCK = 66,
        CASE_HP_MEM_UNLOCK = 67,
        CASE_HP_BUGGY_LL = 68,
        CASE_HPA = 69,
        CASE_VPA = 70,
        CASE_XTERM_WINOPS = 71,
        CASE_ECH = 72,
        CASE_CHT = 73,
        CASE_CPL = 74,
        CASE_CNL = 75,
        CASE_CBT = 76,
        CASE_SU = 77,
        CASE_SD = 78,
        CASE_S7C1T = 79,
        CASE_S8C1T = 80,
        CASE_ESC_SP_STATE = 81,
        CASE_ENQ = 82,
        CASE_DECSCL = 83,
        CASE_DECSCA = 84,
        CASE_DECSED = 85,
        CASE_DECSEL = 86,
        CASE_DCS = 87,
        CASE_PM = 88,
        CASE_SOS = 89,

        /// <summary>
        /// String Terminator
        /// ST is used as the closing delimiter of a control string opened by APPLICATION PROGRAM COMMAND (APC), DEVICE CONTROL STRING (DCS), OPERATING SYSTEM COMMAND 
        /// (OSC), PRIVACY MESSAGE(PM), or START OF STRING(SOS).
        /// </summary>
        CASE_ST = 90,
        CASE_APC = 91,
        CASE_EPA = 92,
        CASE_SPA = 93,
        CASE_CSI_QUOTE_STATE = 94,
        CASE_DSR = 95,
        CASE_ANSI_LEVEL_1 = 96,
        CASE_ANSI_LEVEL_2 = 97,
        CASE_ANSI_LEVEL_3 = 98,
        CASE_MC = 99,
        CASE_DEC2_STATE = 100,
        CASE_DA2 = 101,
        CASE_DEC3_STATE = 102,
        CASE_DECRPTUI = 103,
        CASE_VT52_CUP = 104,
        CASE_REP = 105,
        CASE_CSI_EX_STATE = 106,
        CASE_DECSTR = 107,
        CASE_DECDHL = 108,
        CASE_DECSWL = 109,
        CASE_DECDWL = 110,
        CASE_DEC_MC = 111,
        CASE_ESC_PERCENT = 112,
        CASE_UTF8 = 113,
        CASE_CSI_TICK_STATE = 114,
        CASE_DECELR = 115,
        CASE_DECRQLP = 116,
        CASE_DECEFR = 117,
        CASE_DECSLE = 118,
        CASE_CSI_IGNORE = 119,
        CASE_VT52_IGNORE = 120,
        CASE_VT52_FINISH = 121,
        CASE_CSI_DOLLAR_STATE = 122,
        CASE_DECCRA = 123,
        CASE_DECERA = 124,
        CASE_DECFRA = 125,
        CASE_DECSERA = 126,
        CASE_DECSACE = 127,
        CASE_DECCARA = 128,
        CASE_DECRARA = 129,
        CASE_CSI_STAR_STATE = 130,
        CASE_SET_MOD_FKEYS = 131,
        CASE_SET_MOD_FKEYS0 = 132,
        CASE_HIDE_POINTER = 133,
        CASE_SCS1A_STATE = 134,
        CASE_SCS2A_STATE = 135,
        CASE_SCS3A_STATE = 136,
        CASE_CSI_SPACE_STATE = 137,
        CASE_DECSCUSR = 138,
        CASE_SM_TITLE = 139,
        CASE_RM_TITLE = 140,
        CASE_DECSMBV = 141,
        CASE_DECSWBV = 142,
        CASE_DECLL = 143,
        CASE_DECRQM = 144,
        CASE_RQM = 145,
        CASE_CSI_DEC_DOLLAR_STATE = 146,
        CASE_SL = 147,
        CASE_SR = 148,
        CASE_DECDC = 149,
        CASE_DECIC = 150,
        CASE_DECBI = 151,
        CASE_DECFI = 152,
        CASE_DECRQCRA = 153,
        CASE_HPR = 154,
        CASE_VPR = 155,
        CASE_ANSI_SC = 156,
        CASE_ANSI_RC = 157,
        CASE_ESC_COLON = 158,
        CASE_SCS_PERCENT = 159,
        CASE_GSETS_PERCENT = 160,
        CASE_GRAPHICS_ATTRIBUTES = 161,




        /// <summary>
        /// 换行
        /// 创建一个新行
        /// </summary>
        CASE_LF = 162,

        /// <summary>
        /// 垂直制表符
        /// 它的作用是让‘\v’后面的字符从下一行开始输出，且开始的列数为“\v”前一个字符所在列后面一列。
        /// 例如：puts("01\v2345"),
        /// 01在第一行，占用两列。2345在第二行，并且2在第三列
        /// 
        /// 8.3.161 VT - LINE TABULATION
        /// </summary>
        CASE_VT = 163,

        /// <summary>
        /// 换页（‘\f’）       
        /// 换页符的在终端的中的效果相当于*nix中clear命令。
        /// 终端在输出‘\f’之后内容之前，会将整个终端屏幕清空空，然后在输出内容。
        /// 给人的该觉是在clear命令后的输出字符串。 最后我想说明一点，‘\r’‘\t’‘\v’‘\f’也是控制字符，它们会控制字符的输出方式。
        /// 它们在终端输出时会有上面的表现，但如果写入文本文件，一般文本编辑器（vi或记事本）对‘\r’‘\v’‘\f’的显示是没有控制效果的。
        /// 
        /// 8.3.51 FF - FORM FEED
        /// </summary>
        CASE_FF = 164,
    }
}
