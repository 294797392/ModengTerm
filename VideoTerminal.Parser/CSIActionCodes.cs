using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    public enum CSIActionCodes
    {
        ICH_InsertCharacter = '@',
        CUU_CursorUp = 'A',
        CUD_CursorDown = 'B',
        CUF_CursorForward = 'C',
        CUB_CursorBackward = 'D',
        CNL_CursorNextLine = 'E',
        CPL_CursorPrevLine = 'F',
        CHA_CursorHorizontalAbsolute = 'G',
        CUP_CursorPosition = 'H',
        CHT_CursorForwardTab = 'I',
        ED_EraseDisplay = 'J',
        EL_EraseLine = 'K',
        IL_InsertLine = 'L',
        DL_DeleteLine = 'M',
        DCH_DeleteCharacter = 'P',
        SU_ScrollUp = 'S',
        SD_ScrollDown = 'T',
        ECH_EraseCharacters = 'X',
        CBT_CursorBackTab = 'Z',
        HPA_HorizontalPositionAbsolute = '`',
        HPR_HorizontalPositionRelative = 'a',
        REP_RepeatCharacter = 'b',
        DA_DeviceAttributes = 'c',
        //DA2_SecondaryDeviceAttributes = '>c',
        //DA3_TertiaryDeviceAttributes = '=c',
        VPA_VerticalLinePositionAbsolute = 'd',
        VPR_VerticalPositionRelative = 'e',
        HVP_HorizontalVerticalPosition = 'f',
        TBC_TabClear = 'g',
        //DECSET_PrivateModeSet = '?h',
        //DECRST_PrivateModeReset = '?l',
        SGR_SetGraphicsRendition = 'm',
        DSR_DeviceStatusReport = 'n',
        DECSTBM_SetScrollingRegion = 'r',
        ANSISYSSC_CursorSave = 's', // NOTE: Overlaps with DECLRMM/DECSLRM. Fix when/if implemented.
        DTTERM_WindowManipulation = 't', // NOTE: Overlaps with DECSLPP. Fix when/if implemented.
        ANSISYSRC_CursorRestore = 'u',
        DECREQTPARM_RequestTerminalParameters = 'x',
        //DECSCUSR_SetCursorStyle = ' q',
        //DECSTR_SoftReset = '!p',
        //XT_PushSgrAlias = '#p',
        //XT_PopSgrAlias = '#q',
        //XT_PushSgr = '#{',
        //XT_PopSgr = '#}',
        //DECSCPP_SetColumnsPerPage = '$|',
    }
}
